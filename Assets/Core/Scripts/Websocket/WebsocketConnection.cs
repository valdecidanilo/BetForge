using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Core.Scripts.Data;
using Newtonsoft.Json;
using OPAGames.WebSocket;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Scripts.Websocket
{
    public class WebsocketConnection : MonoBehaviour, IMessageBroker
    {
        private WebSocket _websocket;
        private bool IsOpen => _websocket is { State: WebSocketState.Open };
        private bool IsConnecting => _websocket is { State: WebSocketState.Connecting };
        public Endpoint Endpoint { get; set; }
        
        public event Action OnOpenEvent;
        public event Action OnCloseEvent;
        public event Action<string> OnErrorEvent;
        public event Action<byte, string> OnMessageEvent;
        
        #region Unity Build-In Methods

#if !UNITY_WEBGL || UNITY_EDITOR
        private void Update()
        {
            if (IsOpen) _websocket.DispatchMessageQueue();
        }
#endif
        
        private async void OnDestroy()
        {
            if (IsOpen) await Disconnect();
        }
        
        #endregion
        
        #region IMessageBroker
        
        public Task Connect()
        {
            if (IsOpen)
            {
                OnErrorEvent?.Invoke("You Already Connected");
                return Task.CompletedTask;
            }

            if (IsConnecting)
            {
                OnErrorEvent?.Invoke("You current connecting...");
                return Task.CompletedTask;
            }

            //Get the token from the server
            var token = Endpoint["playerSession"] ?? Endpoint["user_token"] ?? string.Empty;
            var server = Endpoint["server_url"] ?? "null";
            var platform = Endpoint["platform_token"] ?? Endpoint["platform_id"] ?? string.Empty;
            var casinoId = Endpoint["casino_id"] ?? Endpoint["operator"] ?? string.Empty;
            var gameId = Endpoint["game_token"] ?? Endpoint["game_id"] ?? Endpoint["gameId"] ?? string.Empty;
            var language = Endpoint["language"] ?? Endpoint["lang"] ?? "pt";
            
            var cashierUrl = Endpoint["cashierUrl"] ?? Endpoint["cashierurl"] ?? "null";
            var backUrl = Endpoint["lobbyUrl"] ?? Endpoint["lobbyurl"] ?? Endpoint["backUrl"] ?? Endpoint["backurl"] ?? Endpoint["homeurl"] ?? Endpoint["homeUrl"] ?? "null";
            var gameHistoryUrl = Endpoint["gamehistoryurl"] ?? Endpoint["historyUrl"] ?? "null";

            GameData.SetData("lang", language is "pt" or "pt-BR" ? "pt-BR" : "en-US");
            GameData.SetData("endpoint", Endpoint.GetBaseEndpoint(string.Empty, "server_url").Replace("/ws/", string.Empty));
            GameData.SetData("userToken", token);
            GameData.SetData("gameHistoryUrl", gameHistoryUrl);
            GameData.SetData("cashierUrl", cashierUrl);
            GameData.SetData("backUrl", backUrl);
            GameData.SetData("gameId", gameId);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=green>[Opening][WebSocket] {server}?user_token={token}&platform_token={platform}&casino_id={casinoId}&game_id={gameId}</color>");
#endif
            _websocket = new WebSocket($"{server}?user_token={token}&platform_token={platform}&casino_id={casinoId}&game_id={gameId}");

            _websocket.OnOpen += OnOpen;
            _websocket.OnClose += OnClose;
            _websocket.OnError += OnError;
            _websocket.OnMessage += OnMessage;
            
            _websocket.Connect();
            
            return Task.CompletedTask;
        }

        public async Task Disconnect()
        {
            if (!IsOpen)
            {
                OnErrorEvent?.Invoke("You are not connected");
                return;
            }

            await _websocket.Close();

            _websocket.OnOpen -= OnOpen;
            _websocket.OnClose -= OnClose;
            _websocket.OnError -= OnError;
            _websocket.OnMessage -= OnMessage;
        }

        public async Task Send(int command, object obj)
        {
            var raw = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(raw);
            var bytes = new byte[data.Length + 1];

#if UNITY_EDITOR
            Debug.Log($"<color=cyan>[command] {command}>></color> {raw}");
#endif

            bytes[0] = (byte)command;
            Buffer.BlockCopy(data, 0, bytes, 1, data.Length);
            await Send(bytes);
        }

        public Task Get<T>(string route, Action<T> onSuccess, Action<string> onError)
        {
            var url = Endpoint.GetBaseEndpoint(String.Empty, "server_url").Replace("/ws/", string.Empty) + route;
            StartCoroutine(GetRequest(url, onSuccess, onError));
            return Task.CompletedTask;
        }
        
        public Task Post<T>(string route, object data, Action<T> onSuccess, Action<string> onError)
        {
            var url = Endpoint.GetBaseEndpoint(String.Empty, "server_url").Replace("/ws/", string.Empty) + route;
            StartCoroutine(PostRequest(url, JsonConvert.SerializeObject(data), onSuccess, onError));
            return Task.CompletedTask;
        }
        
        #endregion
        
        private void OnOpen()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=green>[Open][WebSocket]</color>");
#endif
            OnOpenEvent?.Invoke();
        }

        private void OnClose(WebSocketCloseCode closeCode) => OnCloseEvent?.Invoke();
        private void OnError(string errorMsg) => OnErrorEvent?.Invoke(errorMsg);
        private void OnMessage(byte[] message)
        {
            var eventType = message[0];
            var data = new byte[message.Length - 1];
            Buffer.BlockCopy(message, 1, data, 0, data.Length);
            var json = Encoding.UTF8.GetString(data);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"<color=#53E066>[event][raw] {eventType} <<</color> {json}");
#endif
            OnMessageEvent?.Invoke(eventType, json);
        }
        
        public static T ParseEvent<T>(byte[] bytes, JsonSerializerSettings settings = null) where T : struct
        {
            var raw = Encoding.UTF8.GetString(bytes);
            return settings == null ? JsonConvert.DeserializeObject<T>(raw) : JsonConvert.DeserializeObject<T>(raw, settings);
        }
        
        private static IEnumerator GetRequest<T>(string route, Action<T> onSuccess, Action<string> onError)
        {
            const int maxRetries = 3; 
            var attempts = 0;
            while (attempts < maxRetries)
            {
                using var request = UnityWebRequest.Get(route);
                request.timeout = 5;
                
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    Debug.Log($"<color=green>[GET] {route}\n {request.downloadHandler.text}</color>");
#endif
                    onSuccess?.Invoke(data);
                    yield break;
                }
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log($"<color=green>[GET][ERROR] {route}\n {request.error}</color>");
#endif

                onError?.Invoke(request.error);

                attempts++;
                if (attempts < maxRetries)
                {
                    yield return new WaitForSeconds(attempts * 1.5f);
                }
            }
        }
        
        private static IEnumerator PostRequest<T>(string route, string data, Action<T> onSuccess, Action<string> onError = null)
        {
            var request = new UnityWebRequest(route, "POST");
            
            var bodyRaw = Encoding.UTF8.GetBytes(data);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);

            request.SetRequestHeader("Content-Type", "application/json");

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
                onSuccess?.Invoke(response);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
        
        private Task Send(byte[] data)
        {
            if (!IsOpen)
            {
                OnErrorEvent?.Invoke("You are not connected!");
                return Task.CompletedTask;
            }

            var task = _websocket.Send(data);
            return task;
        }
        
    }
}