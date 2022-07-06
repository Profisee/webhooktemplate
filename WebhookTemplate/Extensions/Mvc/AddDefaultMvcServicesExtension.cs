//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Profisee.WebhookTemplate.Extensions.Mvc
{
    internal static class AddDefaultMvcServicesExtension
    {
        public static IServiceCollection AddDefaultMvcServices(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                var convention = new RouteTokenTransformerConvention(new CamelCaseRoutingConvention());
                options.Conventions.Add(convention);
            });

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // For Swagger
                    options.SuppressMapClientErrors = true;
                });

            return services;
        }
    }
}