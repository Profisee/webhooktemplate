using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.WebApp.Clients.Entities;
using Profisee.WebhookTemplate.WebApp.Clients.Records;

namespace Profisee.WebhookTemplate.WebApp.Clients.Extensions
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
