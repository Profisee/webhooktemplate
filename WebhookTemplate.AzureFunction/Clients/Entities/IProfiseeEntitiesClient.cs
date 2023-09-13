using Profisee.WebhookTemplate.AzureFunction.Clients.Entities.Responses;
using System;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.AzureFunction.Clients.Entities
{
    public interface IProfiseeEntitiesClient
    {
        Task<GetEntityResponse> GetEntityAsync(Guid entityUId, string authorizationHeader);
    }
}