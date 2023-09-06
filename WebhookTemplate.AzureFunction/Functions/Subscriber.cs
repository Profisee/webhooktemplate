using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.Common.Clients;
using Profisee.WebhookTemplate.Common.Clients.Dtos;
using Profisee.WebhookTemplate.Common.Clients.Entities.Responses;
using Profisee.WebhookTemplate.Common.Configuration;
using Profisee.WebhookTemplate.Common.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebhookTemplate.AzureFunction.Payloads;

namespace WebhookTemplate.AzureFunction.Functions;

public class Subscriber
{
    private static HttpClient client;
    private static IConfigurationRoot config;

    static Subscriber()
    {
        config = new ConfigurationBuilder()
                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();

        client = new HttpClient();
        var url = config.GetValue<string>("ServiceUrl") + "rest/";
        client.BaseAddress = new Uri(url);
    }

    [FunctionName("Subscriber")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
         ILogger log)
    {
        string msg;
        try
        {
            msg = $"Subsciber HTTP trigger function processed a request.";

            log.LogInformation(msg);

            string authorizationHeader = req.Headers["Authorization"];
            log.LogInformation(authorizationHeader);
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                msg = "Authorization header is null";
                log.LogInformation(msg);
                return new OkResult();
            }
            authorizationHeader = authorizationHeader.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                log.LogInformation("NO Authorization header");
                return new OkResult();
            }
            else
            {
                log.LogInformation($"Authorization header: {authorizationHeader}");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var discoveryDocument = await getDiscoveryDocument();
            try
            {
                tokenHandler.ValidateToken(authorizationHeader,
                getTokenValidationParameters(discoveryDocument),
                out SecurityToken validatedToken);
            }
            catch (Exception ex)
            {
                msg = "Token failed validation";
                log.LogInformation(msg);
                return new UnauthorizedResult();
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                authorizationHeader);

            using var reader = new StreamReader(req.Body);
            string requestBody = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                msg = "Body is null or empty";
                log.LogInformation(msg);
                return new OkObjectResult(msg);
            }

            log.LogInformation($"Body: {requestBody}");

            SubscriberPayload data = JsonConvert.DeserializeObject<SubscriberPayload>(requestBody);

            log.LogInformation($"EntityObject: {data.EntityObject}");
            log.LogInformation($"MemberCode: {data.MemberCode}");
            log.LogInformation($"Transaction: {data.Transaction}");
            log.LogInformation($"UserName: {data.UserName}");
            log.LogInformation($"EventName: {data.EventName}");

            updateDescriptionFromRequest(data, req, config, log);

            return new OkResult();
        }
        catch (Exception ex)
        {
            log.LogInformation(ex.Message);
            log.LogInformation(ex.StackTrace);
            msg = ex.Message;
            return new OkObjectResult(msg);
        }
    }

    private async Task<DiscoveryDocumentResponse> getDiscoveryDocument()
    {
        return await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            
            Address = config.GetValue<string>("ServiceUrl") + "auth",
            
            Policy = { RequireHttps = false, ValidateIssuerName = false }
       
        });
    }

    public TokenValidationParameters getTokenValidationParameters(DiscoveryDocumentResponse disco)
    {
        var keys = new List<SecurityKey>();
        foreach (var webKey in disco.KeySet.Keys)
        {
            var e = Base64Url.Decode(webKey.E);
            var n = Base64Url.Decode(webKey.N);

            var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
            {
                KeyId = webKey.Kid
            };

            keys.Add(key);
        }

        var parameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidIssuer = disco.Issuer,

            IssuerSigningKeys = keys,

            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role,

            RequireSignedTokens = true
        };

        return parameters;
    }

    private async void updateDescriptionFromRequest(SubscriberPayload payload, HttpRequest req, IConfigurationRoot config, ILogger log)
    {
        var appsettings = config.Get<AppSettings>();

        var getEntityResponse = await getEntityAsync(payload.EntityObject.Id);

        if (!getEntityResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(getEntityResponse);

            log.LogInformation($"Error: {errorDto}");

        }

        var description = $"{getEntityResponse.Entity.Identifier.Name} - {payload.MemberCode} was updated by the Webhook " +
            $"using the Profisee Rest API at {DateTime.Now}";
        var attributeNameValuePair = new Dictionary<string, object>
        {
            {"Description", description}
        };

        var updateRecordResponse = await updateRecordAsync(payload.EntityObject.Id, payload.MemberCode, attributeNameValuePair);

        if (!updateRecordResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(updateRecordResponse);

            log.LogInformation($"Error: {errorDto}");
        }
    }

    private async Task<GetEntityResponse> getEntityAsync(Guid entityUId)
    {
        const string uriFormat = "v{0}/Entities/{1}";
        var requestUri = string.Format(uriFormat, 1, entityUId);

        var response = await getAsync(requestUri);

        var getEntityResponse = new GetEntityResponse
        {
            Success = response.Success,
            StatusCode = response.StatusCode,
            Message = response.Message,
        };

        if (response.Success && response.HasContent)
        {
            getEntityResponse.Entity = JsonConvert.DeserializeObject<EntityDto>(response.Content);
        }

        return getEntityResponse;
    }

    private async Task<ProfiseeContentResponse> getAsync(string requestUri)
    {
        using (var response = await client.GetAsync(requestUri))
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var retValue = new ProfiseeContentResponse
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = response.ReasonPhrase,
                Content = responseContent
            };

            return retValue;
        }
    }

    private ErrorDto getErrorFromProfiseeResponse(ProfiseeResponse profiseeResponse)
    {
        var errorDto = new ErrorDto
        {
            Code = (int)profiseeResponse.StatusCode,
            Message = profiseeResponse.Message
        };

        return errorDto;
    }

    private async Task<ProfiseeContentResponse> updateRecordAsync(Guid entityUId,
            string recordCode,
            Dictionary<string, object> attributeNameValuePairs)
    {
        const string uriFormat = "v{0}/Records/{1}/{2}";
        var requestUri = string.Format(uriFormat, 1, entityUId, recordCode);

        var result = await patchAsync(requestUri, attributeNameValuePairs);

        return result;
    }

    private async Task<ProfiseeContentResponse> patchAsync(string requestUri, object content)
    {
        var settings = new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };
        var json = JsonConvert.SerializeObject(content, settings);

        using (var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json))
        using (var response = await client.PatchAsync(requestUri, stringContent))
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var retValue = new ProfiseeContentResponse
            {
                Success = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Message = response.ReasonPhrase,
                Content = responseContent
            };

            return retValue;
        }
    }
}