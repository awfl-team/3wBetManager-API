using System;
using Nest;

namespace Proxy
{
    public class ElasticSearchClient
    {
        private static ElasticSearchClient _instance;
        private readonly ElasticClient _elasticClient;

        private ElasticSearchClient()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"));

            _elasticClient = new ElasticClient(settings);
        }

        public static ElasticSearchClient Instance => _instance ?? (_instance = new ElasticSearchClient());

        public async void PublishToElastic(string index, object document)
        {
            await _elasticClient.IndexAsync(document, idx => idx.Index(index));
        }
    }
}