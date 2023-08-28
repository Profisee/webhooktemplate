using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WebhookTemplate.WebApp.Swashbuckle
{
    public class AuthorizeHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                && !context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (!isAuthorized)
            {
                return;
            }

            var unAuthorizedResponse = new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() };
            operation.Responses.TryAdd(HttpStatusCode.Unauthorized.ToString("D"), unAuthorizedResponse);

            var forbiddenResponse = new OpenApiResponse { Description = HttpStatusCode.Forbidden.ToString() };
            operation.Responses.TryAdd(HttpStatusCode.Forbidden.ToString("D"), forbiddenResponse);

            var securityScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
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

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [securityScheme] = Array.Empty<string>()
                }
            };
        }
    }
}