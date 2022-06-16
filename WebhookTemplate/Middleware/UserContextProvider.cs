//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Profisee.WebhookTemplate.Contexts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.Middleware
{
    /// <summary>
    /// Middleware to populate UserContext class during the request pipeline,
    /// making it available to other services while handling a request. One possible
    /// use case of the this Middleware and the UserContext it populates is that the
    /// UserContext's SecurityToken could then be used in the auth header for subsequent
    /// calls to the Profisee REST API.
    /// </summary>
    public sealed class UserContextProvider
    {
        private readonly ILogger<UserContextProvider> logger;
        private readonly RequestDelegate next;

        public UserContextProvider(ILogger<UserContextProvider> logger,
            RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                CreateUserContext(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError($"Unauthorized access error while trying to read token data: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while trying to read token data: {ex.Message}");
            }

            await next(context);
        }

        private void CreateUserContext(HttpContext context)
        {
            // Attempt to fetch JWT from auth header
            var tokenString = context
                .Request
                .Headers["Authorization"]
                .ToString();

            if (string.IsNullOrWhiteSpace(tokenString))
            {
                tokenString = context.Request.Query["token"].ToString();
            }

            // If we can't get an auth token, return
            if (string.IsNullOrWhiteSpace(tokenString))
            {
                return;
            }

            tokenString = tokenString.Replace("Bearer ", string.Empty);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Initializing user context {@tokenString}", tokenString);

            // Initialize user context with JWT
            var userContext = (UserContext)context
                .RequestServices
                .GetService(typeof(UserContext));

            var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(tokenString);
            userContext.UpdateSecurityToken(securityToken);
        }
    }
}