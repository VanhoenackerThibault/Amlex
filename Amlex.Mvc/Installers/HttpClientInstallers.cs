using Amlex.Repository;
using Amlex.Services.Sparql.Settings;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Mvc.Installers
{
    public static class HttpClientInstallers
    {
        public static void InstallHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient("SPARQL", options => {
                options.BaseAddress = new Uri(SparqlSettings.ApiEndpoint);
            });
        }
    }
}
