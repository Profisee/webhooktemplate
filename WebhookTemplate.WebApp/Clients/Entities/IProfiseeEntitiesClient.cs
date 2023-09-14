using Profisee.WebhookTemplate.WebApp.Clients.Entities.Responses;
using System;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Clients.Entities
{
    public interface IProfiseeEntitiesClient : IBaseProfiseeClient
    {
        Task<GetEntityResponse> GetEntityAsync(Guid entityUId);
    }
}