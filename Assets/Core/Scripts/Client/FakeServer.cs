using System;
using System.Threading.Tasks;

namespace Core.Scripts.Websocket
{
    public class FakeServer : IMessageBroker
    {
        public event Action OnOpenEvent;
        public event Action OnCloseEvent;
        public event Action<string> OnErrorEvent;
        public event Action<byte, string> OnMessageEvent;
        public Task Connect()
        {
            OnOpenEvent?.Invoke();
            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task Send(int command, object obj)
        {
            throw new NotImplementedException();
        }

        public Task Get<T>(string route, Action<T> onSuccess, Action<string> onError)
        {
            throw new NotImplementedException();
        }

        public Task Post<T>(string route, object data, Action<T> onSuccess, Action<string> onError)
        {
            throw new NotImplementedException();
        }
    }
}