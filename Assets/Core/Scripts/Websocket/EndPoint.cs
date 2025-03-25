using System;
using System.Collections.Specialized;
using System.Web;
using JetBrains.Annotations;

namespace Core.Scripts.Websocket
{
    [Serializable]
    public struct QueryParam
    {
        public string key;
        public string value;

        public QueryParam(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [Serializable]
    public struct Endpoint
    {
        private Uri _uri;
        private NameValueCollection _queryParams;
        public string this[string key] => _queryParams[key];

        public Endpoint(string uri)
        {
            _uri = new Uri(uri);
            _queryParams = new NameValueCollection();

            QueriesToParam();
        }

        public string GetBaseEndpoint([CanBeNull] string scheme, [CanBeNull] string key)
        {
            var endpoint = this[key];
            if (endpoint.StartsWith("ws://")) return $"http://{endpoint.Replace("ws://", string.Empty)}";

            return endpoint.StartsWith("wss://")
                ? $"https://{endpoint.Replace("wss://", string.Empty)}"
                : $"{scheme}://{endpoint}";
        }

        private void QueriesToParam()
        {
            if (_uri.Query.Length <= 0) return;
            _queryParams = HttpUtility.ParseQueryString(_uri.Query);
        }
    }
}