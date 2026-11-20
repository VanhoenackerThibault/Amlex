using Amlex.Interfaces.Services;
using Amlex.Models.Core;
using Amlex.Models.Mappers;
using Amlex.Models.Models.LegalActs;
using Amlex.Models.Models.SparqlModels;
using Amlex.Services.Sparql.Settings;
using Azure;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Amlex.Services.Sparql
{
    public class SparqlApiService(IHttpClientFactory factory, ILogger<SparqlApiService> logger, ILegalActService legalActService, ILegalActsFetchHistoryService legalActsFetchHistoryService, ILegalActRelationService legalActRelationService, ILegalActTopicService legalActTopicService)
    {
        private async Task<SparqlResponse> GetSparqlDataAsync(FormUrlEncodedContent content)
        {
            HttpClient client = factory.CreateClient("SPARQL");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
            var response = await client.PostAsync(SparqlSettings.ApiEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();

                logger.LogError("A problem occurred while adding backlog: status code {StatusCode}, response body: {ErrorBody}", response.StatusCode, errorBody);

                throw new InvalidOperationException($"CELLAR SPARQL request failed with status code {(int)response.StatusCode}.");
            }

            SparqlResponse? data = await response.Content.ReadFromJsonAsync<SparqlResponse>();

            if (data is null)
            {
                logger.LogError("A problem has occured while fetching data");
                throw new InvalidOperationException("CELLAR SPARQL returned a successful response, but its JSON payload could not be deserialized.");
            }

            return data;
        }


        public async Task RunBackFillSparqlDataAsync(int pageSize = 200, int maxPages = 1)
        {
            int currentPage = 0;
            DateTime? lastModifiedCursor = null;

            while (currentPage < maxPages)
            {
                try
                {

                    // fetching base info to create legalact
                    FormUrlEncodedContent content = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("query", SparqlSettings.BuildDiscoveryQuery(pageSize, before: lastModifiedCursor))
                    ]);

                    SparqlResponse data = await GetSparqlDataAsync(content);

                    List<LegalAct> acts = data.Results.Bindings.ToEntityList();

                    if (acts is null || acts.Count == 0)
                    {
                        break;
                    }

                    await legalActService.CreateLegalActsAsync(acts);

                    // fetching relation info to link tables
                    FormUrlEncodedContent contentRelations = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("query", SparqlSettings.BuildRelationsQuery(acts.Select(a => a.Celex)))
                    ]);

                    SparqlResponse relationData = await GetSparqlDataAsync(contentRelations);
                    List<LegalAct> dbLegalActs = await legalActService.GetLegalActsByListCelexAsync(acts.Select(a => a.Celex).ToList());

                    List<LegalActRelation> legalActRelations = relationData.Results.Bindings.ToRelationEntities(
                            dbLegalActs.ToDictionary(legalAct => legalAct.Celex, legalAct => legalAct.Id));

                    await legalActRelationService.CreateLegalActRelationAsync(legalActRelations);


                    // fetching topics
                    FormUrlEncodedContent contentTopics = new FormUrlEncodedContent(
                    [
                        new KeyValuePair<string, string>("query", SparqlSettings.BuildEuroVocQuery(acts.Select(a => a.Celex)))
                    ]);

                    SparqlResponse TopicData = await GetSparqlDataAsync(contentTopics);

                    List<LegalActTopic> legalActTopics = TopicData.Results.Bindings.ToTopicEntities(
                            dbLegalActs.ToDictionary(legalAct => legalAct.Celex, legalAct => legalAct.Id));

                    await legalActTopicService.CreateLegalActTopic(legalActTopics);

                    await legalActsFetchHistoryService.CreateWatchItemAsync(acts);


                    lastModifiedCursor = acts.Min(d => d.LastModifiedAt);

                    currentPage++;

                }
                catch (Exception ex)
                {
                    logger.LogError("A problem has occured while getting backfill message: {message}", ex);
                    break;
                }

            }
        }


        public async Task UpdateSparqlData()
        {
            const int PageSize = 200;
            DateTime? lastFetchTime = await legalActsFetchHistoryService.GetLastFetchTimeAsync();

            if (lastFetchTime is not null && DateTime.UtcNow < lastFetchTime.Value.AddHours(1))
            {
                logger.LogWarning("UpdateSparqlData called but fetched data less than 1 hour ago, returning");
                return;
            }

            logger.LogInformation("Getting new info out of cellar database");

            DateTime? lastUpdateTime = await legalActsFetchHistoryService.GetLastUpdateTimeAsync();


            // fetching base info to create legalact
            FormUrlEncodedContent content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("query", SparqlSettings.BuildDiscoveryQuery(PageSize, since: lastUpdateTime))
            ]);

            SparqlResponse data = await GetSparqlDataAsync(content);

            List<LegalAct> acts = data.Results.Bindings.ToEntityList();

            if (acts is null || acts.Count == 0)
            {
                logger.LogWarning("No acts found");
                return;
            }

            await legalActService.CreateLegalActsAsync(acts);

            // fetching relation info to link tables
            FormUrlEncodedContent contentRelations = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("query", SparqlSettings.BuildRelationsQuery(acts.Select(a => a.Celex)))
            ]);

            SparqlResponse relationData = await GetSparqlDataAsync(contentRelations);
            List<LegalAct> dbLegalActs = await legalActService.GetLegalActsByListCelexAsync(acts.Select(a => a.Celex).ToList());

            List<LegalActRelation> legalActRelations = relationData.Results.Bindings.ToRelationEntities(
                    dbLegalActs.ToDictionary(legalAct => legalAct.Celex, legalAct => legalAct.Id));

            await legalActRelationService.CreateLegalActRelationAsync(legalActRelations);


            // fetching topics
            FormUrlEncodedContent contentTopics = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("query", SparqlSettings.BuildEuroVocQuery(acts.Select(a => a.Celex)))
            ]);

            SparqlResponse TopicData = await GetSparqlDataAsync(contentTopics);

            List<LegalActTopic> legalActTopics = TopicData.Results.Bindings.ToTopicEntities(
                    dbLegalActs.ToDictionary(legalAct => legalAct.Celex, legalAct => legalAct.Id));

            await legalActTopicService.CreateLegalActTopic(legalActTopics);

            await legalActsFetchHistoryService.CreateWatchItemAsync(acts);
        }
    }
}
