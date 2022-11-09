//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Profisee.WebhookTemplate.Services;

namespace Profisee.WebhookTemplate.Extensions.Authentication
{
    internal static class AddProfiseeAuthorizationExtension
    {
        public static IServiceCollection AddProfiseeAuthorization(this IServiceCollection services)
        {
            services
                .Replace(ServiceDescriptor.Transient<IAuthorizationService, ProfiseeAuthorizationService>());

            return services;
        }
    }
}