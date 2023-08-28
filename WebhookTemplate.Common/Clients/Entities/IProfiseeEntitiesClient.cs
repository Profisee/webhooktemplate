using Profisee.WebhookTemplate.Common.Clients.Entities.Responses;

namespace Profisee.WebhookTemplate.Common.Clients.Entities
{
    public interface IProfiseeEntitiesClient : IBaseProfiseeClient
    {
        Task<GetEntityResponse> GetEntityAsync(Guid entityUId);
    }
}