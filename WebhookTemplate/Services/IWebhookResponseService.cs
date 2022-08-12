//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Profisee.WebhookTemplate.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.Services
{
    public interface IWebhookResponseService
    {
        Task<WebhookResponseDto> ProcessRequest(Dictionary<string, object> data);

        Task<WebhookResponseDto> ProcessRequest(WebhookRequestDto dto);

        Task ProcessRequest(SubscriberPayloadDto dto);

        Task<WebhookResponseDto> UpdateDescriptionFromRequest(WebhookRequestDto dto);
    }
}