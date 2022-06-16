//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Profisee.WebhookTemplate.Extensions.Swagger
{
    internal static class AddSwaggerDefaultsExtension
    {
        public static IServiceCollection AddSwaggerDefaults(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VV";
                    options.SubstituteApiVersionInUrl = true;
                })
                .AddSwaggerGen(options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>();
                    options.OperationFilter<AuthorizeHeaderOperationFilter>();

                    var securityScheme = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Authorization",
                        Description = "Specify the authorization token.",
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http
                    };

                    options.AddSecurityDefinition("Bearer", securityScheme);
                })
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();

            return services;
        }
    }
}
