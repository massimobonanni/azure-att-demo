
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SecretAndConfig.WebSite
{
    public class Program
    {

        public static IHostEnvironment CurrentEnvironment;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    CurrentEnvironment = hostingContext.HostingEnvironment;

                    config.AddJsonFile(
                        "appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.local.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.local.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    #region [ AppConfiguration environment]
                    if (hostingContext.HostingEnvironment.IsAppConfigurationEnvironment())
                    {
                        var settings = config.Build();
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(settings["ConnectionStrings:AppConfig"])
                                   .UseFeatureFlags();
                        });
                    }
                    #endregion [ AppConfiguration environment]
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
