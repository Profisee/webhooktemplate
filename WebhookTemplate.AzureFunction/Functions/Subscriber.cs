using IdentityModel.Client;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.Common.Dtos;
using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Profisee.WebhookTemplate.Common.Configuration;
using Profisee.WebhookTemplate.Common.Clients.Dtos;
using Profisee.WebhookTemplate.Common.Clients.Entities.Responses;
using Profisee.WebhookTemplate.Common.Clients;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebhookTemplate.AzureFunction.Functions;

public class Subscriber
{
    [FunctionName("Subscriber")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
         ILogger log)
    {
        var config = new ConfigurationBuilder()
                 .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables()
                 .Build();
        var client = new HttpClient(this.getHttpHandler());
        var url = config.GetValue<string>("ServiceUrl") + "rest/";
        client.BaseAddress = new Uri(url);
        string msg;
        try
        {
             msg = $"ActivityGeneric HTTP trigger function processed a request.";

            log.LogInformation(msg);

            msg = $"Call came  from - {req.Headers["X-Forwarded-For"]}";
            log.LogInformation(msg);

            foreach (var item in req.Headers)
            {
                log.LogInformation($"Header = {item.Key} Value = {item.Value} \n");
            }

            string authorizationHeader = req.Headers["Authorization"];
            log.LogInformation(authorizationHeader);
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                msg = "Authorization header is null";
                log.LogInformation(msg);
                return new OkObjectResult(msg);
            }
            authorizationHeader = authorizationHeader.Remove(0, 7);

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                log.LogInformation("NO Authorization header");
            }
            else
            {
                log.LogInformation($"Authorization header: {authorizationHeader}");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var d = getDisco(config, client);
            tokenHandler.ValidateToken(authorizationHeader, getTokenValidationParameters(d), out SecurityToken validatedToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                 authorizationHeader);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                log.LogInformation($"Body is null or empty");
                return new OkObjectResult(msg);
            }

            log.LogInformation($"Body: {requestBody}");

            Payload data = JsonConvert.DeserializeObject<Payload>(requestBody);
            log.LogInformation($"EntityObject: {data.EntityObject}");
            log.LogInformation($"MemberCode: {data.MemberCode}");
            log.LogInformation($"Transaction: {data.Transaction}");
            log.LogInformation($"UserName: {data.UserName}");
            log.LogInformation($"EventName: {data.EventName}");
            var webhookResponse = UpdateDescriptionFromRequest(data, req, client, config, log);

            return new OkResult();
        }
        catch (Exception ex)
        {
            log.LogInformation(ex.StackTrace);
            log.LogInformation(ex.Message);
            msg = ex.Message;
        }

        return new OkObjectResult(msg);

    }
    public DiscoveryDocumentResponse getDisco(IConfigurationRoot config, HttpClient client)
    {
        return client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {

            Address = config.GetValue<string>("ServiceUrl") + "auth",
            Policy = { RequireHttps = false, ValidateIssuerName = false }
        }).Result;
    }
    private HttpClientHandler getHttpHandler()
    {
        var handler = new HttpClientHandler { UseDefaultCredentials = true };
        return handler;
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
    public class Payload
    {
        public Identifier EntityObject { get; set; }
        public string MemberCode { get; set; }
        public int Transaction { get; set; }
        public string UserName { get; set; }
        public string EventName { get; set; }
    }

    public class Identifier
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}  Name: {this.Name}";
        }
    }
    public class SubscriberResponse
    {
        [JsonProperty(Required = Required.Default)]
        public int ProcessingStatus { get; set; }
        
        [JsonProperty(Required = Required.Default)]
        public Dictionary<string, object> ResponsePayload { get; set; }
    }
    public async Task<SubscriberResponse> UpdateDescriptionFromRequest(Payload payload, HttpRequest req, HttpClient client, IConfigurationRoot config, ILogger log)
    {
        var response = new SubscriberResponse();
        var appsettings = config.Get<AppSettings>();

        var getEntityResponse = await GetEntityAsync(payload.EntityObject.Id, client);

        if (!getEntityResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(getEntityResponse);

            response.ResponsePayload.Add("Error", errorDto);
            response.ProcessingStatus = -1;

            return response;
        }

        var description = $"{getEntityResponse.Entity.Identifier.Name} - {payload.MemberCode} was updated by the Webhook using the Profisee Rest API at {DateTime.Now}";
        var attributeNameValuePair = new Dictionary<string, object>
        {
            {"Description", description}
        };

        var updateRecordResponse = await UpdateRecordAsync(payload.EntityObject.Id, payload.MemberCode, attributeNameValuePair, client);

        if (!updateRecordResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(updateRecordResponse);

            response.ResponsePayload.Add("Error", errorDto);
            response.ProcessingStatus = -1;
        }

        return response;
    }
    public async Task<GetEntityResponse> GetEntityAsync(Guid entityUId, HttpClient client)
    {
        const string uriFormat = "v{0}/Entities/{1}";
        var requestUri = string.Format(uriFormat, 1, entityUId);

        var response = await GetAsync(requestUri, client);

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
    protected async Task<ProfiseeContentResponse> GetAsync(string requestUri, HttpClient client)
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
    public async Task<ProfiseeContentResponse> UpdateRecordAsync(Guid entityUId,
            string recordCode,
            Dictionary<string, object> attributeNameValuePairs,
            HttpClient client)
    {
        const string uriFormat = "v{0}/Records/{1}/{2}";
        var requestUri = string.Format(uriFormat, 1, entityUId, recordCode);

        var result = await PatchAsync(requestUri, attributeNameValuePairs, client);

        return result;
    }
    protected async Task<ProfiseeContentResponse> PatchAsync(string requestUri, object content, HttpClient client)
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