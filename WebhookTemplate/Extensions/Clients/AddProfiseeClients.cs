//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.Clients.Entities;
using Profisee.WebhookTemplate.Clients.Records;

namespace Profisee.WebhookTemplate.Extensions.Authentication
{
    internal static class AddProfiseeClientsExtension
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