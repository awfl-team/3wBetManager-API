using System;
using System.Diagnostics;

namespace Proxy
{
    public class ElasticsSearchDatabaseContext : IDisposable
    {
        private readonly string _collection;
        private readonly string _method;
        private readonly Stopwatch _stopwatch;

        public ElasticsSearchDatabaseContext(string method, string collection)
        {
            _method = method;
            _collection = collection;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            ElasticSearchClient.Instance.PublishToElastic("database", new
            {
                CreatedAt = DateTime.Now,
                Method = _method,
                Collection = _collection,
                EllapsedTime = _stopwatch.ElapsedMilliseconds
            });
        }
    }
}