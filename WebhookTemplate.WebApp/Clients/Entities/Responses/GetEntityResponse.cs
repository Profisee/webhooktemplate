using Profisee.WebhookTemplate.WebApp.Clients.Dtos;

namespace Profisee.WebhookTemplate.WebApp.Clients.Entities.Responses
{
    public class GetEntityResponse : ProfiseeResponse
    {
        public EntityDto Entity { get; set; }
    }
}