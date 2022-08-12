//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.Threading.Tasks;
using Profisee.WebhookTemplate.Clients.Entities.Responses;

namespace Profisee.WebhookTemplate.Clients.Entities
{
    internal interface IProfiseeEntitiesClient : IBaseProfiseeClient
    {
        Task<GetEntityResponse> GetEntityAsync(Guid entityUId);
    }
}