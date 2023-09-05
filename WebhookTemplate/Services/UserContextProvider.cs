using Microsoft.AspNetCore.Http;
using Profisee.WebhookTemplate.Common.Contexts;
using Profisee.WebhookTemplate.Common.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Profisee.WebhookTemplate.WebApp.Services
{
    public class UserContextProvider : IUserContextProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserContext userContext;

        public UserContextProvider(IHttpContextAccessor httpContextAccessor, UserContext userContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userContext = userContext;
        }

        public UserContext GetUserContext()
        {
            var request = httpContextAccessor.HttpContext.Request;

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
