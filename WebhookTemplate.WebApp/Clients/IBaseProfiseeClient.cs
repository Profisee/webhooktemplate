namespace Profisee.WebhookTemplate.WebApp.Clients
{
    public interface IBaseProfiseeClient
    {
        void SetAuthorizationHeader(string value);
    }
}