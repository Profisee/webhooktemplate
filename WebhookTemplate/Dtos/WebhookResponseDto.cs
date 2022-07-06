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
        /// <summary>
        /// The workflow activity can receive a ProcessingStatus code as part of the return payload.
        /// The meaning of the ProcessingStatus value is up to the workflow designer.
        /// </summary>
        [DataMember]
        public int ProcessingStatus { get; set; }

        /// <summary>
        /// The workflow activity can also receive a ResponsePayload, which is simply a string-object
        /// dictionary. How the workflow uses the values included in the ResponsePayload is up to the
        /// workflow designer.
        /// </summary>
        [DataMember]
        public Dictionary<string, object> ResponsePayload { get; set; }

        public WebhookResponseDto()
        {
            ResponsePayload = new Dictionary<string, object>();
        }
    }
}
