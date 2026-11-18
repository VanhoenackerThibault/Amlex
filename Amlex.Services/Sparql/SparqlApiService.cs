using Amlex.Models.Models.SparqlModels;
using Amlex.Services.Sparql.Settings;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Amlex.Services.Sparql
{
    public class SparqlApiService(IHttpClientFactory factory, ILogger<SparqlApiService> logger)
    {
        //public async Task<SparqlResponse?> GetSparqlDataAsync()
        //{
        //    HttpClient client = factory.CreateClient("SPARQL");

        //    var content = new FormUrlEncodedContent(
        //    [
        //        new KeyValuePair<string, string>("query", SparqlSettings.BuildQuery())
        //    ]);

        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
        //    var response = await client.PostAsync(SparqlSettings.ApiEndpoint, content);

        //    response.EnsureSuccessStatusCode();

        //    return await response.Content.ReadFromJsonAsync<SparqlResponse>();
        //}

        public async Task<List<SparqlBinding>?> RunBackFillSparqlDataAsync(int pageSize = 500, int maxPages = 1)
        {
            HttpClient client = factory.CreateClient("SPARQL");

            int currentPage = 0;
            DateTime? lastModifiedCursor = null;

            List<SparqlBinding> sparqlData = new List<SparqlBinding>();

            while (currentPage < maxPages)
            {
                var content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>("query", SparqlSettings.BuildQuery(pageSize, before: lastModifiedCursor))
                ]);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
                var response = await client.PostAsync(SparqlSettings.ApiEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync();

                    logger.LogError(
                        "A problem occurred while adding backlog: status code {StatusCode}, response body: {ErrorBody}",
                        response.StatusCode,
                        errorBody);

                    continue;
                }

                SparqlResponse? data = await response.Content.ReadFromJsonAsync<SparqlResponse>();

                if (data?.Results.Bindings is null || data.Results.Bindings.Count == 0)
                {
                    logger.LogInformation("CELLAR backfill finished: no bindings on page {Page}", currentPage + 1);
                    break;
                }

                sparqlData.AddRange(data.Results.Bindings);

                logger.LogInformation("CELLAR backfill page {Page} returned {Count} bindings", currentPage + 1, data.Results.Bindings.Count);

                if (data.Results.Bindings.Count < pageSize)
                {
                    break;
                }

                lastModifiedCursor = data.Results.Bindings
                    .Where(d => d.LastModified?.Value is not null)
                    .Min(d => DateTime.Parse(d.LastModified!.Value, CultureInfo.InvariantCulture));

                currentPage++;
            }

            return sparqlData;
        }

    }
}
