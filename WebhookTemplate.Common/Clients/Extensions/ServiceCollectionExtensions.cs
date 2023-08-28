using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.Common.Clients.Entities;
using Profisee.WebhookTemplate.Common.Clients.Records;

namespace Profisee.WebhookTemplate.Common.Clients.Extensions
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
