using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace WebSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                         "appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.local.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile(
                        "appsettings.local.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    //var settings = config.Build();
                    //config.AddAzureAppConfiguration(settings["ConnectionStrings:AppConfig"]);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
