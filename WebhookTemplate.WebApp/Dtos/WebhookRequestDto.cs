using System;
using System.Runtime.Serialization;

namespace Profisee.WebhookTemplate.WebApp.Dtos
{
    /// <summary>
    /// Simple Data Transfer Object (dto) for receiving a typed webhook request payload.
    /// </summary>
    [DataContract]
    public class WebhookRequestDto
    {
        /// <summary>
        /// For the workflow configured for this example, the webhook activity sends a payload with the id of the entity.
        /// </summary>
        [DataMember]
        public Guid EntityId { get; set; }

        /// <summary>
        /// For the workflow configured for this example, the webhook activity sends a payload with the code of the entity.
        /// </summary>
        [DataMember]
        public string Code { get; set; }
    }
}
