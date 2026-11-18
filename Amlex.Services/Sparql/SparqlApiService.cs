using Amlex.Models.Models.SparqlModels;
using Amlex.Services.Sparql.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Amlex.Services.Sparql
{
    public class SparqlApiService(IHttpClientFactory factory)
    {
        public async Task<SparqlResponse?> GetSparqlData()
        {
            HttpClient client = factory.CreateClient("SPARQL");

            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("query", SparqlSettings.BuildQuery())
            ]);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
            var response = await client.PostAsync(SparqlSettings.ApiEndpoint, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<SparqlResponse>();
        }
    }
}
