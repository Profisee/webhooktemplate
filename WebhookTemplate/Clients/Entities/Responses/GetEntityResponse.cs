//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Profisee.WebhookTemplate.Clients.Dtos;

namespace Profisee.WebhookTemplate.Clients.Entities.Responses
{
    internal class GetEntityResponse : ProfiseeResponse
    {
        public EntityDto Entity { get; set; }
    }
}