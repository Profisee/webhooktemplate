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

public class WorkflowUpdateEntityDescription
{
    private static HttpClient client;
    private static IConfigurationRoot config;

    static WorkflowUpdateEntityDescription()
    {
        config = new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        client = new HttpClient();
        var url = config.GetValue<string>("ServiceUrl") + "rest/";
        client.BaseAddress = new Uri(url);
    }

    [FunctionName("WorkflowUpdateEntityDescription")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
         ILogger log)
    {
        var message = $"Workflow Update Entity HTTP trigger function processed a request.";

        try
        {
            log.LogInformation(message);

            // Extract the authorization header from the incoming HTTP request.
            string authorizationHeader = req.Headers["Authorization"];

            log.LogInformation(authorizationHeader);
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                message = "Authorization header is null";
                log.LogInformation(message);
                return new OkObjectResult(message);
            }

            // Remove the "Bearer " prefix from the authorization header. This automatically gets added back in when validating the JWT.
            authorizationHeader = authorizationHeader.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                message = "NO Authorization header";
                log.LogInformation(message);
                return new OkObjectResult(message);
            }
            else
            {
                log.LogInformation($"Authorization header: {authorizationHeader}");
            }

            // Create a JwtSecurityTokenHandler to validate the JWT token.
            var tokenHandler = new JwtSecurityTokenHandler();
            var discoveryDocument = await getDiscoveryDocument();
            try
            {
                // Validate the JWT token using the provided discovery document.
                tokenHandler.ValidateToken(authorizationHeader,
                    getTokenValidationParameters(discoveryDocument),
                    out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                message = "Token failed validation";
                log.LogInformation(message);
                return new UnauthorizedResult();
            }

            // Set the default authorization header for the HttpClient.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                authorizationHeader);

            var reader = new StreamReader(req.Body);
            string requestBody = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                message = "Body is null or empty";
                log.LogInformation(message);
                return new OkObjectResult(message);
            }

            log.LogInformation($"Body: {requestBody}");

            var data = JsonConvert.DeserializeObject<WorkflowPayload>(requestBody);

            log.LogInformation($"EntityObject: {data.EntityObject}");
            log.LogInformation($"MemberCode: {data.MemberCode}");

            // Update the description using the request data.
            var webhookResponse = updateDescriptionFromRequest(data, log);

            message = webhookResponse.Status.ToString();
            return new OkObjectResult(message);
        }
        catch (Exception ex)
        {
            log.LogInformation(ex.StackTrace);
            log.LogInformation(ex.Message);
            message = ex.Message;
            return new OkObjectResult(message);
        }
    }

    // Method to retrieve the discovery document for token validation.
    private async Task<DiscoveryDocumentResponse> getDiscoveryDocument()
    {
        return await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {

            Address = config.GetValue<string>("ServiceUrl") + "auth",
            Policy = { RequireHttps = false, ValidateIssuerName = false }
        });
    }

    // Method to create TokenValidationParameters for JWT token validation.
    public TokenValidationParameters getTokenValidationParameters(DiscoveryDocumentResponse disco)
    {
        // Create a list of security keys from the discovery document.
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

        // Configure token validation parameters.Here, you can set a clock skew in order to handle any JWT validation issues related to timing.
        var parameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidIssuer = disco.Issuer,

            IssuerSigningKeys = keys,

            NameClaimType = JwtClaimTypes.Name,
            RoleClaimType = JwtClaimTypes.Role,

            RequireSignedTokens = true,
            
            //To solve JWT token validation issues related to clock skew, you can change this parameter to longer/shorter timespans
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        return parameters;
    }

    // Method to update the description based on the request data.
    private async Task<WorkflowWebhookResponse> updateDescriptionFromRequest(WorkflowPayload payload, ILogger log)
    {
        var response = new WorkflowWebhookResponse();

        var getEntityResponse = await getEntityAsync(payload.EntityObject.Id);

        if (!getEntityResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(getEntityResponse);

            log.LogInformation($"Error: {errorDto}");
        }

        var description = payload.Description;
        var attributeNameValuePair = new Dictionary<string, object>
        {
            {"Description", description}
        };

        var updateRecordResponse = await updateRecordAsync(payload.EntityObject.Id, payload.MemberCode, attributeNameValuePair);

        if (!updateRecordResponse.Success)
        {
            var errorDto = getErrorFromProfiseeResponse(updateRecordResponse);

            response.ResponsePayload.Add("Error", errorDto);
            response.ProcessingStatus = -1;
        }

        return response;
    }

    //Method to retrieve an entity directly from the Profisee Service.
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

    //Method to send the Http Get Request to the Profisee Service.
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

    //Method to handle any Profisee Error Responses.
    private ErrorDto getErrorFromProfiseeResponse(ProfiseeResponse profiseeResponse)
    {
        var errorDto = new ErrorDto
        {
            Code = (int)profiseeResponse.StatusCode,
            Message = profiseeResponse.Message
        };

        return errorDto;
    }

    //Method to update the records in the Profisee Service.
    private async Task<ProfiseeContentResponse> updateRecordAsync(Guid entityUId,
        string recordCode,
        Dictionary<string, object> attributeNameValuePairs)
    {
        const string uriFormat = "v{0}/Records/{1}/{2}";
        var requestUri = string.Format(uriFormat, 1, entityUId, recordCode);

        var result = await PatchAsync(requestUri, attributeNameValuePairs, client);

        return result;
    }

    //Method to make an Http Patch Request to the Profisee Service.
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