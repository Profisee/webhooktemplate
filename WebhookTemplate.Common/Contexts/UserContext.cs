using System.IdentityModel.Tokens.Jwt;

namespace Profisee.WebhookTemplate.Common.Contexts
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
    }
}