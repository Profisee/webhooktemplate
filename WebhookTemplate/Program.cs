//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Entry point of the application. Creates the web host.
            var host = buildHost(args);

            try
            {
                // Run the web host.
                await host.RunAsync();
            }
            finally
            {
                // Ensure any logs still in the Serilog buffer are written out before shutting down.
                Log.CloseAndFlush();

                // When the web host is finished running, dispose of
                // all resources it's holding
                host.Dispose();
            }
        }

        private static IHost buildHost(string[] args)
        {
            // Create the configuration from the local appsettings
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Create the web host object. Integrate with Serilog,
            // specify the IConfiguration object, and integrate with IIS
            var host = Host
                .CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseConfiguration(configuration);
                    webHostBuilder.UseStartup<Startup>();
                    webHostBuilder.UseIIS();
                })
                .Build();

            return host;
        }
    }
}
