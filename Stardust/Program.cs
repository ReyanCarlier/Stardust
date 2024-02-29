using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Stardust.Database.Context;
using Stardust.Database.CRUD;
using Stardust.Pages.Admin;

namespace Stardust
{
    public class Program
    {
        private static string nmapScriptFolder = "";
        public static bool GlobalScanPending { get; set; } = false;

        public static string? MySQLConnectionString { get; set; } = null;
        public static string? NmapScriptFolder { get => nmapScriptFolder; set => nmapScriptFolder = value ?? ""; }

        public static void Main(string[] args)
        {
            WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

            if (builder == null)
                return;
            NmapScriptFolder = builder?.Configuration?["Nmap:ScriptFolder"];
            if (NmapScriptFolder == null || NmapScriptFolder == "")
                return;

            IEnumerable<string>? initialScopes = builder?.Configuration["GraphApi:Scopes"]?.Split(' ');
            _ = (builder?.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes));
            _ = (builder?.Services.AddMicrosoftGraphBeta(builder.Configuration.GetSection("GraphApi"))
                    .AddInMemoryTokenCaches());
            _ = (builder?.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }));

            _ = (builder?.Services.AddScoped<GraphWrapper.GraphWrapper>());

            _ = (builder?.Services.AddDistributedMemoryCache());
            _ = (builder?.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            }));
            _ = (builder?.Services.AddRazorPages()
                  .AddMicrosoftIdentityUI());
            builder?.Logging.AddFilter((category, level) =>
            {
                return level >= LogLevel.Warning;
            });
            var connectionString = builder?.Configuration.GetConnectionString("Stardust");

            _ = (builder?.Services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler());
            _ = (builder?.Services.AddDbContext<StardustContext>(options => options.UseSqlite(connectionString)));
            _ = (builder?.Services.AddScoped<StardustCRUD>());
            _ = (builder?.Services.AddSingleton<BackgroundGlobalScanService>());
            _ = (builder?.WebHost.UseStaticWebAssets());
            _ = (builder?.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.Secure = CookieSecurePolicy.Always;
            }));
            _ = (builder?.Services.AddMudServices());
            MySQLConnectionString = builder?.Configuration.GetConnectionString("MySQL");

            var app = builder?.Build();
            if (app == null)
                return;
            if (!app.Environment.IsDevelopment())
            {
                _ = app.UseExceptionHandler("/Error");
                _ = app.UseHsts();
            }

            _ = app.UseHttpsRedirection();
            _ = app.UseStaticFiles();
            _ = app.UseRouting();
            _ = app.UseCookiePolicy();
            _ = app.UseAuthentication();
            _ = app.UseAuthorization();
            _ = app.MapControllers();
            _ = app.MapBlazorHub();
            _ = app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}