using Amlex.Interfaces.Services;
using Amlex.Services.Database;
using Amlex.Services.Sparql;

namespace Amlex.Mvc.Installers
{
    public static class ServiceInstaller
    {
        public static void InstallScopedServices(this IServiceCollection services)
        {
            services.AddScoped<SparqlApiService>();
            services.AddScoped<ILegalActService, LegalActService>();
            services.AddScoped<ILegalActsFetchHistoryService, LegalActsFetchHistoryService>();
            services.AddScoped<ILegalActRelationService, LegalActRelationService>();
            services.AddScoped<ILegalActTopicService, LegalActTopicService>();
            services.AddScoped<IEuroVocCodesService, EuroVocCodesService>();
        }
    }
}
