using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Profisee.WebhookTemplate.AzureFunction.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WebhookTemplate.AzureFunction.Clients.Auth
{
    public class ProfiseeAuthClient : IProfiseeAuthClient
    {
        private readonly HttpClient httpClient;
        private readonly Uri serviceUrl;
        private readonly ILogger logger;
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        public ProfiseeAuthClient(HttpClient httpClient, IOptions<AppSettings> appSettings, ILogger logger)
        {
            this.httpClient = httpClient;
            this.serviceUrl = appSettings.Value.ServiceUrl;
            this.logger = logger;

            this.jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<bool> validateJwt(string token)
        {
            var discoveryDocument = await getDiscoveryDocumentAsync();

            if (discoveryDocument.IsError)
            {
                // log error
                return false;
            }

            var tokenValidationParameters = getTokenValidationParameters(discoveryDocument);

            try
            {
                jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception exception)
            {
                // log error
                return false;
            }
        }

        private async Task<DiscoveryDocumentResponse> getDiscoveryDocumentAsync()
        {
            var requestUri = serviceUrl.ToString() + "auth";

            var request = new DiscoveryDocumentRequest
            {
                Address = requestUri,
                Policy =
                {
                    RequireHttps = false,
                    ValidateIssuerName = false
                }
            };

            var response = await this.httpClient.GetDiscoveryDocumentAsync(request);

            return response;
        }

        private TokenValidationParameters getTokenValidationParameters(DiscoveryDocumentResponse discoveryDocument)
        {
            // Create a list of security keys from the discovery document.
            var keys = new List<SecurityKey>();
            foreach (var webKey in discoveryDocument.KeySet.Keys)
            {
                var e = Base64Url.Decode(webKey.E);
                var n = Base64Url.Decode(webKey.N);

                var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
                {
                    KeyId = webKey.Kid
                };

                keys.Add(key);
            }

            // Configure token validation parameters.Here, you can set a clock skew in order to handle any JWT validation issues related to timing.
            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidIssuer = discoveryDocument.Issuer,

                IssuerSigningKeys = keys,

                NameClaimType = JwtClaimTypes.Name,
                RoleClaimType = JwtClaimTypes.Role,

                RequireSignedTokens = true,

                //To solve JWT token validation issues related to clock skew, you can change this parameter to longer/shorter timespans
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            return parameters;
        }
    }
}
