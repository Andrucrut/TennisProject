using Infrastructure.Data.Entities;
using Interfaces.Interfaces;
using Nest;

namespace TennisProject
{
    public class ElasticSearchService:ILogService
    {
        private readonly IElasticClient client;

        public ElasticSearchService()
        {
            var settings = new ConnectionSettings(new Uri("http://5.101.5.102:9300")).BasicAuthentication("elastic", "dio9Yai@gu")
                .DefaultIndex("logbackend");
                
            client = new ElasticClient(settings);
        }
        

        public async Task<bool> AddLog(Log document)
        {
            var response = await client.IndexDocumentAsync(document);

            if (!response.IsValid)
            {
                throw new Exception($"Ошибка индексации документа: {response.OriginalException.Message}");
            }
            return true;
        }

        public async Task<ISearchResponse<Log>> SearchDocumentsAsync(SearchDescriptor<Log> searchDescriptor) 
        {
            var searchResponse = await client.SearchAsync<Log>(searchDescriptor);

            if (!searchResponse.IsValid)
            {
                throw new Exception($"Ошибка поиска: {searchResponse.OriginalException.Message}");
            }

            return searchResponse;
        }
    }
}
