using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.AzureFunction.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.AzureFunction.Clients
{
    public abstract class BaseProfiseeClient
    {
        private static readonly JsonSerializerSettings settings;

        static BaseProfiseeClient()
        {
            settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };
        }

        protected readonly HttpClient httpClient;
        protected readonly IOptions<AppSettings> appSettings;
        protected readonly ILogger logger;

        public BaseProfiseeClient(HttpClient httpClient, IOptions<AppSettings> appSettings, ILogger logger)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.httpClient = httpClient;
        }

        protected async Task<ProfiseeContentResponse> PatchAsync(string requestUri, object content, string authorizationHeader)
        {
            var json = JsonConvert.SerializeObject(content, settings);

            using var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            requestUri = requestUri.Insert(0, appSettings.Value.ServiceUrl + "rest/");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Patch, requestUri);

            requestMessage.Content = stringContent;
            requestMessage.Headers.Authorization 
                = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authorizationHeader);

            using var response = await this.httpClient.SendAsync(requestMessage);

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

        protected async Task<ProfiseeContentResponse> GetAsync(string requestUri, string authorizationHeader)
        {
            requestUri = requestUri.Insert(0, appSettings.Value.ServiceUrl + "rest/");
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            requestMessage.Headers.Authorization
                = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, authorizationHeader);

            using var response = await this.httpClient.SendAsync(requestMessage);

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
