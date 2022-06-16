//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq;
using System.Reflection;

namespace Profisee.WebhookTemplate.Extensions.Swagger
{
    internal static class UseSwaggerDefaultsExtension
    {
        public static IApplicationBuilder UseSwaggerDefaults(
            this IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var assembly = Assembly.GetExecutingAssembly();

                var description = provider.ApiVersionDescriptions.First();
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);

                options.RoutePrefix = string.Empty;

                options.IndexStream = () => assembly.GetManifestResourceStream("WebhookTemplate.wwwroot.swashbuckle.index.html");

                options.InjectStylesheet("../swashbuckle/styles.css");

                options.DocExpansion(DocExpansion.Full);

                options.InjectJavascript("../swashbuckle/custom.js");

                // Disable validator messages on Swagger page
                // options.ValidatorUrl(null);
            });

            return app;
        }
    }
}
