using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.AzureFunction.Clients.Records
{
    public interface IProfiseeRecordsClient
    {
        Task<ProfiseeContentResponse> UpdateRecordAsync(Guid entityUId, 
            string recordCode, 
            Dictionary<string, object> attributeNameValuePairs, 
            string authorizationHeader);
    }
}