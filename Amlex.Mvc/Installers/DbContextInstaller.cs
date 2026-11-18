using Amlex.Repository;
using Microsoft.EntityFrameworkCore;

namespace Amlex.Mvc.Installers
{
    public static class DbContextInstaller
    {
        public static void InstallDbConnection(this IServiceCollection services, string? conn = null)
        {
            // if no conn is given switch over to inmemory database
            if (conn is null)
            {
                services.AddDbContext<AmlexDbContext>(options =>
                {
                    options.UseInMemoryDatabase(nameof(AmlexDbContext));

                });
            }
            else
            {
                services.AddDbContext<AmlexDbContext>(options =>
                {
                    options.UseSqlServer(conn);
                });
            }
        }
    }
}
