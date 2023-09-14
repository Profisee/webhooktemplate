using System.Threading.Tasks;

namespace WebhookTemplate.AzureFunction.Clients.Auth
{
    public interface IProfiseeAuthClient
    {
        Task<bool> validateJwt(string token);
    }
}