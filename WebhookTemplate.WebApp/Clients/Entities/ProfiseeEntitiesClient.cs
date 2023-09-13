using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.WebApp.Clients.Dtos;
using Profisee.WebhookTemplate.WebApp.Clients.Entities.Responses;
using Profisee.WebhookTemplate.WebApp.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Clients.Entities
{
    public class ProfiseeEntitiesClient : BaseProfiseeClient, IProfiseeEntitiesClient
    {
        private static readonly int apiVersion = 1;

        public ProfiseeEntitiesClient(HttpClient httpClient, IOptions<AppSettings> appSettings, ILogger<ProfiseeEntitiesClient> logger)
            : base(httpClient, appSettings, logger)
        {

        }

        public async Task<GetEntityResponse> GetEntityAsync(Guid entityUId)
        {
            const string uriFormat = "v{0}/Entities/{1}";
            var requestUri = string.Format(uriFormat, apiVersion, entityUId);

            var response = await base.GetAsync(requestUri);

            var getEntityResponse = new GetEntityResponse
            {
                Success = response.Success,
                StatusCode = response.StatusCode,
                Message = response.Message,
            };

            if (response.Success && response.HasContent)
            {
                getEntityResponse.Entity = JsonConvert.DeserializeObject<EntityDto>(response.Content);
            }

            return getEntityResponse;
        }
    }
}
