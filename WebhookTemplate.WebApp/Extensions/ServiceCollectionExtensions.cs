using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Profisee.WebhookTemplate.Common.Configuration;
using Profisee.WebhookTemplate.Common.Contexts;
using Profisee.WebhookTemplate.Common.Services;
using Profisee.WebhookTemplate.WebApp.Services;
using WebhookTemplate.WebApp.Mvc;

namespace Profisee.WebhookTemplate.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebhookTemplateServices(this IServiceCollection services)
        {
            services.AddTransient<IWebhookResponseService, WebhookResponseService>();

            return services;
        }

        public static IServiceCollection AddUserContextServices(this IServiceCollection services)
        {
            services.AddScoped<UserContext>();
            services.AddTransient<IUserContextProvider, UserContextProvider>();

            return services;
        }

        public static IServiceCollection AddProfiseeAuthorization(this IServiceCollection services)
        {
            services
                .Replace(ServiceDescriptor.Transient<IAuthorizationService, ProfiseeAuthorizationService>());

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, AppSettings appSettings)
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
