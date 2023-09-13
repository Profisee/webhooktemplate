using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Clients.Records
{
    public interface IProfiseeRecordsClient : IBaseProfiseeClient
    {
        Task<ProfiseeContentResponse> UpdateRecordAsync(Guid entityUId, string recordCode, Dictionary<string, object> attributeNameValuePairs);
    }
}