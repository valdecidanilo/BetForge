using System;
using System.Threading.Tasks;

namespace Core.Scripts.Websocket
{
    public interface IMessageBroker
    {
        public event Action OnOpenEvent;
        public event Action OnCloseEvent;
        public event Action<string> OnErrorEvent;
        public event Action<byte, string> OnMessageEvent;
        
        public Task Connect();

        public Task Disconnect();

        public Task Send(int command, object obj);

        public Task Get<T>(string route, Action<T> onSuccess, Action<string> onError);
        public Task Post<T>(string route, object data, Action<T> onSuccess, Action<string> onError);
    }
}