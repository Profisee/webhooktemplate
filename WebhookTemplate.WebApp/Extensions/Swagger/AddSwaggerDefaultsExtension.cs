//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Profisee.WebhookTemplate.WebApp.Extensions.Swagger
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
                    options.ExampleFilters();

                    var securityScheme = new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Name = "Authorization",
                        Description = "Specify the authorization token.",
                        BearerFormat = "JWT",
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http
                    };

                    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                    var path = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                    options.IncludeXmlComments(path);
                })
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>()
                .AddSwaggerExamplesFromAssemblyOf<Startup>();

            return services;
        }
    }
}
