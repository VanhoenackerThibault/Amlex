using Amlex.Repository;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Mvc.Installers
{
    public static class DbContextInstaller
    {
        public static void InstallDbConnection(this IServiceCollection services)
        {
            // using in memory for debuging
            services.AddDbContext<AmlexDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(AmlexDbContext));

            });
        }
    }
}
