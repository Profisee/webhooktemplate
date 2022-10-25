//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Clients.Records
{
    internal interface IProfiseeRecordsClient : IBaseProfiseeClient
    {
        Task<ProfiseeContentResponse> UpdateRecordAsync(Guid entityUId, string recordCode, Dictionary<string, object> attributeNameValuePairs);
    }
}