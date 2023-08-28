namespace Profisee.WebhookTemplate.Common.Clients
{
    public interface IBaseProfiseeClient
    {
        void SetAuthorizationHeader(string value);
    }
}