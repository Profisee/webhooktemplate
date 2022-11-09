//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Profisee.WebhookTemplate.Extensions.Swagger
{
    internal class SwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider provider;

        public SwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            this.provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var descriptions = this.provider.ApiVersionDescriptions;

            foreach (var description in descriptions)
            {
                var info = new OpenApiInfo
                {
                    Title = "Profisee Sample Webhook",
                    Version = description.ApiVersion.ToString(),
                    Description = "Profisee's Sample Webhook"
                };

                if (description.IsDeprecated)
                {
                    info.Description += " - Deprecated";
                }

                options.SwaggerDoc(description.GroupName, info);
            }
        }
    }
}