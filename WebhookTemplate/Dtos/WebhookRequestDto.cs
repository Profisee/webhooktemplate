//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System.Runtime.Serialization;

namespace Profisee.WebhookTemplate.Dtos
{
    /// <summary>
    /// Simple Data Transfer Object (dto) for receiving a typed webhook request payload.
    /// </summary>
    [DataContract]
    public class WebhookRequestDto
    {
        [DataMember]
        public string Code { get; set; }
    }
}
