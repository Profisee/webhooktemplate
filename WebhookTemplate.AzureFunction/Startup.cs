using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.AzureFunction.Services;
using Profisee.WebhookTemplate.Common.Clients.Extensions;
using Profisee.WebhookTemplate.Common.Configuration;
using Profisee.WebhookTemplate.Common.Contexts;
using Profisee.WebhookTemplate.Common.Services;
using Profisee.WebhookTemplate.WebApp.Services;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Profisee.WebhookTemplate.AzureFunction.Startup))]

namespace Profisee.WebhookTemplate.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<AppSettings>().Configure(appSettings =>
            {
                appSettings.ServiceUrl = new Uri(environmentVariable(nameof(appSettings.ServiceUrl)));
                appSettings.LoggingConnectionString = environmentVariable(nameof(appSettings.LoggingConnectionString));
                appSettings.LoggingFilePath = environmentVariable(nameof(appSettings.LoggingFilePath));
            });

            builder.Services.AddProfiseeClients();
            builder.Services.AddTransient<IUserContextProvider, UserContextProvider>();
            builder.Services.AddTransient<IWebhookResponseService, WebhookResponseService>();
            builder.Services.AddScoped<UserContext>();
            builder.Services.AddHttpContextAccessor();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            var appsettingsPath = Path.Combine(context.ApplicationRootPath, "appsettings.json");

            builder.ConfigurationBuilder
                .AddJsonFile(appsettingsPath, optional: true)
                .AddEnvironmentVariables();
        }

        private static string environmentVariable(string name) => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
