//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.Configuration;

namespace Profisee.WebhookTemplate.Clients
{
    internal abstract class BaseProfiseeClient : IBaseProfiseeClient
    {
        private static readonly JsonSerializerSettings settings;

        static BaseProfiseeClient()
        {
            settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };
        }

        private readonly HttpClient httpClient;
        protected readonly ILogger<BaseProfiseeClient> logger;

        public BaseProfiseeClient(HttpClient httpClient,
            IOptions<AppSettings> appSettings,
            ILogger<BaseProfiseeClient> logger)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri($"{appSettings.Value.ServiceUrl}rest/", UriKind.Absolute);
        }

        public void SetAuthorizationHeader(string value)
        {
            httpClient.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, value);
        }

        protected async Task<ProfiseeContentResponse> PatchAsync(string requestUri, object content)
        {
            var json = JsonConvert.SerializeObject(content, settings);

            using (var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json))
            using (var response = await this.httpClient.PatchAsync(requestUri, stringContent))
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var retValue = new ProfiseeContentResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    Message = response.ReasonPhrase,
                    Content = responseContent
                };

                return retValue;
            }
        }

        protected async Task<ProfiseeContentResponse> GetAsync(string requestUri)
        {
            using (var response = await this.httpClient.GetAsync(requestUri))
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var retValue = new ProfiseeContentResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    Message = response.ReasonPhrase,
                    Content = responseContent
                };

                return retValue;
            }
        }
    }
}
