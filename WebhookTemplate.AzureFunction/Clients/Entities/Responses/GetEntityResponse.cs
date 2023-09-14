using Profisee.WebhookTemplate.AzureFunction.Clients.Dtos;

namespace Profisee.WebhookTemplate.AzureFunction.Clients.Entities.Responses
{
    public class GetEntityResponse : ProfiseeResponse
    {
        public EntityDto Entity { get; set; }
    }
}