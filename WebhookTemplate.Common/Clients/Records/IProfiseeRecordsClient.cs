namespace Profisee.WebhookTemplate.Common.Clients.Records
{
    public interface IProfiseeRecordsClient : IBaseProfiseeClient
    {
        Task<ProfiseeContentResponse> UpdateRecordAsync(Guid entityUId, string recordCode, Dictionary<string, object> attributeNameValuePairs);
    }
}