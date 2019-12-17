using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace TeamManager.Manual
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddAzureWebAppDiagnostics())
                .ConfigureServices(services => services.Configure<AzureBlobLoggerOptions>(options => {
                        options.BlobName = "teammanagerapplogs.txt";
                    }
                ))
                .UseStartup<Startup>();
    }
}
