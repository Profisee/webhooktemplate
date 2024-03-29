using System;

namespace Profisee.WebhookTemplate.WebApp.Clients.Dtos
{
    public class IdentifierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int InternalId { get; set; }
        public bool IsReferenceValid { get; set; }
    }
}
