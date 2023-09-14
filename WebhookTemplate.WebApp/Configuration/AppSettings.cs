using Newtonsoft.Json;
using System;

namespace Profisee.WebhookTemplate.WebApp.Configuration
{
    /// <summary>
    /// Application configuration object that binds to the specified section in StartUp.cs IConfiguration object
    /// </summary>
    public class AppSettings
    {
        public const string Section = "Profisee";

        private Uri? serviceUrl;

        // Optional setting for outputting log information to a file in addition to the default location
        [JsonProperty(Required = Required.AllowNull)]
        public string? LoggingFilePath { get; set; }

        // Optional setting for outputting log information to a database
        [JsonProperty(Required = Required.AllowNull)]
        public string? LoggingConnectionString { get; set; }

        // Uri to Profisee service
        [JsonProperty(Required = Required.Always)]
        public Uri ServiceUrl
        {
            get => serviceUrl!;
            set
            {
                if (!value.ToString().EndsWith('/'))
                {
                    serviceUrl = new Uri(value.ToString() + '/');
                }
                else
                {
                    serviceUrl = value;
                }
            }
        }
    }
}