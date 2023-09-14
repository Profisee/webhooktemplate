using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.AzureFunction.Clients.Entities;
using Profisee.WebhookTemplate.AzureFunction.Clients.Records;

namespace Profisee.WebhookTemplate.AzureFunction.Clients.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProfiseeClients(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<IProfiseeRecordsClient, ProfiseeRecordsClient>();
            services.AddTransient<IProfiseeEntitiesClient, ProfiseeEntitiesClient>();

            return services;
        }
    }
}
