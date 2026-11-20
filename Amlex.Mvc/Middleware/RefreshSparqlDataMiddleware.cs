using Amlex.Interfaces.Services;
using Amlex.Services.Sparql;

namespace Amlex.Mvc.Middleware
{
    public class RefreshSparqlDataMiddleware(RequestDelegate next, ILogger<RefreshSparqlDataMiddleware> logger)
    {

        private static DateTime? LastFetch = null;

        public async Task InvokeAsync(HttpContext httpContext, SparqlApiService sparqlApiService, ILegalActsFetchHistoryService legalActsFetchHistoryService)
        {

            if (LastFetch is null)
            {
                logger.LogInformation("lastfetch is null getting new date");
                LastFetch = await legalActsFetchHistoryService.GetLastFetchTimeAsync();
            }

            if (LastFetch is null || LastFetch.Value.AddHours(1) < DateTime.UtcNow)
            {
                try
                {
                    LastFetch = DateTime.UtcNow;
                    await sparqlApiService.UpdateSparqlData();

                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Failed to refresh SPARQL legal-act data");
                }
            }

            await next(httpContext);
        }
    }


}
