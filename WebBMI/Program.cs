using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace WebBMI
{
    public class Program
    {
        static string AppConfigConnStr = "________AppConfigConnStr_______";
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            //config Azure App Config FeatureManagement
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    var settings = builder.Build();

                    // This section can be used to pull feature flag configuration from Azure App Configuration
                    builder.AddAzureAppConfiguration(o =>
                    {
                        o.Connect(AppConfigConnStr).ConfigureRefresh(
                            refresh =>
                        {
                            refresh.Register("Page:SubTitle", refreshAll: true);
                            refresh.SetCacheExpiration(TimeSpan.FromSeconds(5));
                        }); ;

                        o.Select(KeyFilter.Any);

                        o.UseFeatureFlags(FeatureFlagconfig =>
                        {
                            FeatureFlagconfig.CacheExpirationInterval = TimeSpan.FromSeconds(5);
                        });
                    });
                })
                .UseStartup<Startup>();
        }
    }
}
