using Profisee.WebhookTemplate.Common.Contexts;

namespace Profisee.WebhookTemplate.Common.Services
{
    public interface IUserContextProvider
    {
        UserContext GetUserContext();
    }
}