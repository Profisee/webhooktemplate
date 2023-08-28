using Profisee.WebhookTemplate.Common.Clients.Dtos;

namespace Profisee.WebhookTemplate.Common.Clients.Entities.Responses
{
    public class GetEntityResponse : ProfiseeResponse
    {
        public EntityDto Entity { get; set; }
    }
}