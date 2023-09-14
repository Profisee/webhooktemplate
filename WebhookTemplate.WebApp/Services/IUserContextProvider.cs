using Profisee.WebhookTemplate.WebApp.Contexts;

namespace Profisee.WebhookTemplate.WebApp.Services
{
    public interface IUserContextProvider
    {
        UserContext GetUserContext();
    }
}