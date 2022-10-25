//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Profisee.WebhookTemplate.WebApp.Configuration;

namespace Profisee.WebhookTemplate.WebApp.Extensions.Authentication
{
    internal static class AddJwtAuthenticationExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            AppSettings appSettings)
        {
            var authUrl = $"{appSettings.ServiceUrl}auth/";

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configureOptions =>
                {
                    configureOptions.Audience = "ProfiseeAPI";
                    configureOptions.Authority = authUrl;
                    configureOptions.RequireHttpsMetadata = false;
                });

            return services;
        }
    }
}