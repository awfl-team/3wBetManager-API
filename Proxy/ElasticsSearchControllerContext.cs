using System;
using System.Diagnostics;

namespace Proxy
{
    public class ElasticsSearchControllerContext: IDisposable
    {
        private readonly string _ip;
        private readonly string _requestPath;
        private readonly Stopwatch _stopwatch;
        private readonly string _verb;

        public ElasticsSearchControllerContext(string verb, string requestPath, string ip)
        {
            _verb = verb;
            _requestPath = requestPath;
            _ip = ip;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            ElasticSearchClient.Instance.PublishToElastic("controller", new
            {
                Verb = _verb,
                RequestPath = _requestPath,
                Ip = _ip,
                Time = _stopwatch.ElapsedMilliseconds
            });
        }
    }
}
