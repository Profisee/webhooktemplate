//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profisee.WebhookTemplate.WebApp.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.MSSqlServer;
using System;
using System.IO;

namespace Profisee.WebhookTemplate.WebApp.Extensions.Logging
{
    internal static class AddSerilogLoggingExtension
    {
        private static readonly string defaultLogFilePath;

        static AddSerilogLoggingExtension()
        {
            defaultLogFilePath = Path.Combine(Environment.CurrentDirectory, "Logs", "Log.log");
        }

        public static IServiceCollection AddSerilogLogging(this IServiceCollection services,
            IConfiguration configuration, AppSettings appSettings)
        {
            // Enable Serilog internal logging first.
            new FileInfo(defaultLogFilePath).Directory.Create();

            SelfLog.Enable(message =>
            {
                var output = $"Serilog Selflog: {message}";

                Console.WriteLine(output);

                using (var fileStream = File.OpenWrite(defaultLogFilePath))
                using (var writer = new StreamWriter(fileStream))
                using (var synchronizedWriter = TextWriter.Synchronized(writer))
                {
                    synchronizedWriter.WriteLine(output);
                }
            });

            var logConfiguration = new LoggerConfiguration()
                .ReadFrom
                    .Configuration(configuration)
                .WriteTo
                    .Console()
                .WriteTo
                    .File(defaultLogFilePath, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Infinite);

            var useDatabaseLogging = false;
            var useCustomFileLogging = false;

            var loggingConnectionString = appSettings.LoggingConnectionString;
            if (!string.IsNullOrWhiteSpace(loggingConnectionString))
            {
                logConfiguration.AddProfiseeSqlLogging(loggingConnectionString);
                useDatabaseLogging = true;
            }

            var loggingFilePath = appSettings.LoggingFilePath;
            if (!string.IsNullOrWhiteSpace(loggingFilePath))
            {
                logConfiguration.AddProfiseeFileLogging(loggingFilePath);
                useCustomFileLogging = true;
            }

            var logger = logConfiguration.CreateLogger();
            Log.Logger = logger;

            services.AddLogging(configure =>
            {
                configure.AddSerilog(logger, true);
            });

            if (useDatabaseLogging)
            {
                Log.Logger.Information("Database logging enabled");
            }

            if (useCustomFileLogging)
            {
                Log.Logger.Information("Secondary file logging enabled");
            }

            return services;
        }

        private static void AddProfiseeSqlLogging(this LoggerConfiguration logConfiguration, string loggingConnectionString)
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);

            var sinkOptions = new MSSqlServerSinkOptions
            {
                AutoCreateSqlTable = true,
                SchemaName = "WebhookTemplate",
                TableName = "Log"
            };

            logConfiguration.AuditTo
                .MSSqlServer(
                    connectionString: loggingConnectionString,
                    sinkOptions: sinkOptions,
                    columnOptions: columnOptions);
        }

        private static void AddProfiseeFileLogging(this LoggerConfiguration logConfiguration, string loggingFilePath)
        {
            logConfiguration.WriteTo
                    .File(loggingFilePath, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Infinite);
        }
    }
}