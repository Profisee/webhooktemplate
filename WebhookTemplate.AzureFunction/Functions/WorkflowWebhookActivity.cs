using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.AzureFunction.Clients;
using Profisee.WebhookTemplate.AzureFunction.Clients.Entities;
using Profisee.WebhookTemplate.AzureFunction.Clients.Records;
using Profisee.WebhookTemplate.AzureFunction.Configuration;
using Profisee.WebhookTemplate.AzureFunction.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebhookTemplate.AzureFunction.Clients.Auth;

namespace WebhookTemplate.AzureFunction.Functions;

public class WorkflowWebhookActivity
{
    private static HttpClient httpClient;
    private static IOptions<AppSettings> appSettingsOptions;

    static WorkflowWebhookActivity()
    {
        var appSettings = new AppSettings();
        new ConfigurationBuilder()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build()
            .Bind(appSettings);

        appSettingsOptions = Options.Create(appSettings);

        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri($"{appSettingsOptions.Value.ServiceUrl}rest/", UriKind.Absolute);
    }

    [FunctionName("WorkflowWebhookActivity")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
         ILogger log)
    {
        var message = $"Workflow Webhook Activity HTTP trigger function processed a request.";
        log.LogInformation(message);

        try
        {
            // Extract the authorization header from the incoming HTTP request.
            string authorizationHeader = req.Headers["Authorization"];

            log.LogInformation($"Authorization header: {authorizationHeader}");
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                message = "Authorization header is null";
                log.LogError(message);
                return new UnauthorizedObjectResult(message);
            }

            // Remove the "Bearer " prefix from the authorization header. This automatically gets added back in when validating the JWT.
            authorizationHeader = authorizationHeader.Replace("Bearer ", string.Empty);
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                message = "NO Authorization header";
                log.LogError(message);
                return new UnauthorizedObjectResult(message);
            }

            // Create a profisee auth client and validate the auth
            var profiseeAuthClient = new ProfiseeAuthClient(httpClient, appSettingsOptions, log);
            var isJwtValid = await profiseeAuthClient.validateJwt(authorizationHeader);

            if (!isJwtValid)
            {
                message = "JWT is not valid";
                log.LogError(message);
                return new ForbidResult();
            }

            var profiseeEntitiesClient = new ProfiseeEntitiesClient(httpClient, appSettingsOptions, log);
            var profiseeRecordsClient = new ProfiseeRecordsClient(httpClient, appSettingsOptions, log);

            using var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                message = "Body is null or empty";
                log.LogError(message);
                return new OkObjectResult(message);
            }

            log.LogInformation($"Body: {requestBody}");

            var request = JsonConvert.DeserializeObject<WebhookRequestDto>(requestBody);

            log.LogInformation($"EntityObject: {request.EntityId}");
            log.LogInformation($"MemberCode: {request.Code}");

            // Update the description using the request data.
            var response = new WebhookResponseDto();

            var getEntityResponse = await profiseeEntitiesClient.GetEntityAsync(request.EntityId, authorizationHeader);

            if (!getEntityResponse.Success)
            {
                var errorDto = getErrorFromProfiseeResponse(getEntityResponse);
                response.ResponsePayload.Add("Error", errorDto);
                response.ProcessingStatus = -1;

                log.LogError($"Error: {JsonConvert.SerializeObject(errorDto, Formatting.Indented)}");
                return new OkObjectResult(response);
            }

            var description = $"{getEntityResponse.Entity.Identifier.Name} - {request.Code} " 
                + $"was updated by the Azure Function Webhook using the Profisee Rest API at {DateTime.Now}";

            var attributeNameValuePair = new Dictionary<string, object>
            {
                { "Description", description }
            };

            var updateRecordResponse = await profiseeRecordsClient.UpdateRecordAsync(request.EntityId, 
                request.Code, 
                attributeNameValuePair, 
                authorizationHeader);

            if (!updateRecordResponse.Success)
            {
                var errorDto = getErrorFromProfiseeResponse(updateRecordResponse);

                response.ResponsePayload.Add("Error", errorDto);
                response.ProcessingStatus = -1;

                log.LogError($"Error: {JsonConvert.SerializeObject(errorDto, Formatting.Indented)}");
                return new OkObjectResult(response);
            }

            log.LogInformation(JsonConvert.SerializeObject(response, Formatting.Indented));
            return new OkObjectResult(response);
        }
        catch (Exception ex)
        {
            log.LogError(ex.StackTrace);
            log.LogError(ex.Message);
            message = ex.Message;
            return new OkObjectResult(message);
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
}