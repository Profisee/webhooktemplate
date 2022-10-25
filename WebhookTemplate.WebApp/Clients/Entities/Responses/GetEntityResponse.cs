//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Profisee.WebhookTemplate.WebApp.Clients.Dtos;

namespace Profisee.WebhookTemplate.WebApp.Clients.Entities.Responses
{
    internal class GetEntityResponse : ProfiseeResponse
    {
        public EntityDto Entity { get; set; }
    }
}