using Amlex.Mvc.Installers;
using Amlex.Mvc.Middleware;
using Amlex.Repository;
using Amlex.Repository.Seeders;
using Amlex.Services.Sparql.Settings;
using Microsoft.AspNetCore.Identity;

namespace Amlex.Mvc
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // installing dependecies
            string? conn = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.InstallDbConnection(conn);
            builder.Services.InstallScopedServices();
            builder.Services.InstallHttpClients();
            builder.Services.InstallDefaultIdentity();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RefreshSparqlDataMiddleware>();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<AmlexDbContext>();

                await DatabaseSeeder.SeedDatabase(services);
            }

            app.Run();
        }
    }
}
