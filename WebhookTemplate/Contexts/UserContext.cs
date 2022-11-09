//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Profisee.WebhookTemplate.Contexts
{
    /// <summary>
    /// Encapsulates user information that is captured from the HttpContext during message calls
    /// </summary>
    public class UserContext
    {
        public bool Initialized { get; private set; }

        public JwtSecurityToken SecurityToken { get; private set; }

        public void UpdateSecurityToken(JwtSecurityToken securityToken)
        {
            if (this.Initialized)
            {
                throw new InvalidOperationException();
            }

            this.SecurityToken = securityToken;
            this.Initialized = true;
        }

        public string GetSecurityTokenValue(string type)
        {
            var value = SecurityToken
                .Claims
                .FirstOrDefault(x => x.Type == type)
                ?.Value;

            return value;
        }
    }
}