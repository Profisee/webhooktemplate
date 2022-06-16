//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.Services;

namespace Profisee.WebhookTemplate.Extensions.Services
{
    internal static class AddWebhookTemplateServicesExtension
    {
        public static IServiceCollection AddWebhookTemplateServices(this IServiceCollection services)
        {
            services.AddTransient<IWebhookResponseService, WebhookResponseService>();

            return services;
        }
    }
}