using Amlex.Models.Core;
using Amlex.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Amlex.Mvc.Installers;

public static class IdentityInstaller
{
    public static void InstallDefaultIdentity(this IServiceCollection services)
    {
        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AmlexDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Identity/AccessDenied";
            options.LoginPath = "/Authentication/SignIn";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
            options.SlidingExpiration = true;
        });
    }
}