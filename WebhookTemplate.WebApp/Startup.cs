using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Profisee.WebhookTemplate.WebApp.Clients.Extensions;
using Profisee.WebhookTemplate.WebApp.Configuration;
using Profisee.WebhookTemplate.WebApp.Extensions;
using System;
using WebhookTemplate.WebApp.Extensions;
using WebhookTemplate.WebApp.Swashbuckle;

namespace Profisee.WebhookTemplate.WebApp
{
    internal class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var configSection = this.configuration.GetSection(AppSettings.Section);
            var appsettings = configSection.Get<AppSettings>();

            services
                .Configure<AppSettings>(configSection)          // Adds configuration object to DI container
                .AddSerilogLogging(configuration, appsettings)  // Adds logging
                .AddDefaultMvcServices()                        // Adds default ASPNET Core MVC services
                .AddSwaggerDefaults()                           // Adds Swagger/Swashbuckle configuration
                .AddJwtAuthentication(appsettings)              // Adds JWT authentication
                .AddUserContextServices()                       // Adds user context provided by UserContextProvider middleware
                .AddProfiseeAuthorization()                     // Adds custom authorization handler
                .AddProfiseeClients()                           // Adds typed http clients to communicate with the Profisee REST API
                .AddWebhookTemplateServices();                  // Adds our custom services
        }

        public void Configure(ILogger<Startup> logger,
            IApplicationBuilder appBuilder,
            IWebHostEnvironment webHostEnvironment,
            IApiVersionDescriptionProvider versionDescriptionProvider,
            IServiceProvider serviceProvider)
        {
            if (webHostEnvironment.IsDevelopment())
            {
                // Outputs unhandled exception information
                appBuilder.AddProfiseeExceptionHandler();
            }

            // For Swagger
            appBuilder.UseSwaggerDefaults(versionDescriptionProvider);
            appBuilder.UseStaticFiles();

            // Standard ASP.NET Core configuration calls.
            // Note that UseAuthentication and UseAuthorization must be called
            // between the UseRouting and UseEndpoints calls.
            appBuilder.UseHttpsRedirection();
            appBuilder.UseRouting();
            appBuilder.UseAuthentication();
            appBuilder.UseAuthorization();
            appBuilder.UseEndpoints(options =>
            {
                options.MapControllers();
            });
        }
    }
}
