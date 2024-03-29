using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Net.Mime;
using System.Text;

namespace WebhookTemplate.WebApp.Extensions
{
    /// <summary>
    /// Adds a page that outputs unhandled exception details
    /// </summary>
    internal static class AppBuilderExtensions
    {
        public static void AddProfiseeExceptionHandler(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseExceptionHandler(exceptionHandlerBuilder =>
            {
                var logger = exceptionHandlerBuilder.ApplicationServices.GetService<ILogger>();

                exceptionHandlerBuilder.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Text.Plain;

                    await context.Response.WriteAsync("Exception thrown.");

                    var exceptionHandlerPathFeature = context
                        .Features
                        .Get<IExceptionHandlerPathFeature>();

                    var exception = exceptionHandlerPathFeature.Error;

                    var builder = new StringBuilder();
                    builder.AppendLine();
                    builder.AppendLine("Exception Detail:");
                    builder.AppendLine($"Type: {exception.GetType().Name}");
                    builder.AppendLine($"Message: {exception.Message}");
                    builder.AppendLine($"StackTrace: {exception.StackTrace}");

                    var innerException = exception.InnerException;

                    while (innerException is not null)
                    {
                        builder.AppendLine();
                        builder.AppendLine("Inner Exception Detail:");
                        builder.AppendLine($"Type: {innerException.GetType().Name}");
                        builder.AppendLine($"Message: {innerException.Message}");
                        builder.AppendLine($"StackTrace: {innerException.StackTrace}");

                        innerException = innerException.InnerException;
                    }

                    Log.Logger.Error(builder.ToString());

                    await context.Response.WriteAsync(builder.ToString());
                });
            });
        }
    }
}