//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Profisee.WebhookTemplate.Dtos
{
    /// <summary>
    /// Simple Data Transfer Object (dto) for responding to a web hook request.
    /// </summary>
    [DataContract]
    public class WebhookResponseDto
    {
        [DataMember]
        public int ProcessingStatus { get; set; }

        [DataMember]
        public Dictionary<string, object> ResponsePayload { get; set; }

        public WebhookResponseDto()
        {
            ResponsePayload = new Dictionary<string, object>();
        }
    }
}
