using Microsoft.AspNetCore.Http;
using Profisee.WebhookTemplate.Common.Contexts;
using Profisee.WebhookTemplate.Common.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Profisee.WebhookTemplate.AzureFunction.Services
{
    public class UserContextProvider : IUserContextProvider
    {
        private readonly UserContext userContext;
        private readonly HttpRequest request;

        public UserContextProvider(UserContext userContext, HttpRequest request)
        {
            this.userContext = userContext;
            this.request = request;
        }

        public UserContext GetUserContext()
        {
            // Attempt to fetch JWT from auth header
            var tokenString = request
                .Headers["Authorization"]
                .ToString();

            // If we can't get an auth token, return
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return userContext;
            }

            tokenString = tokenString.Replace("Bearer ", string.Empty);

            //logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Initializing user context {@tokenString}", tokenString);

            var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            userContext.UpdateSecurityToken(securityToken);
            return userContext;
        }
    }
}
