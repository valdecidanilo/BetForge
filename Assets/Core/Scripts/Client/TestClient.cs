using Core.Scripts.Websocket;
using UnityEngine;

namespace Core.Scripts.Client
{
    public class TestClient
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Test()
        {
            IMessageBroker broker = new FakeServer();
            broker.OnOpenEvent += () => Debug.Log("test");
            broker.Connect();
        }
        
    }
}