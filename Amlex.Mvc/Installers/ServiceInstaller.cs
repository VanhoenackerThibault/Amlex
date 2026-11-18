using Amlex.Services.Sparql;

namespace Amlex.Mvc.Installers
{
    public static class ServiceInstaller
    {
        public static void InstallScopedServices(this IServiceCollection services)
        {
            services.AddScoped<SparqlApiService>();
        }
    }
}
