//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.WebApp.Clients;
using Profisee.WebhookTemplate.WebApp.Clients.Entities;
using Profisee.WebhookTemplate.WebApp.Clients.Records;
using Profisee.WebhookTemplate.WebApp.Contexts;
using Profisee.WebhookTemplate.WebApp.Dtos;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Services
{
    /// <summary>
    /// Sample service used to handle incoming webhook requests.
    /// </summary>
    internal class WebhookResponseService : IWebhookResponseService
    {
        private readonly ILogger<IWebhookResponseService> logger;
        private readonly UserContext userContext;
        private readonly IProfiseeEntitiesClient profiseeEntitiesClient;
        private readonly IProfiseeRecordsClient profiseeRecordsClient;

        public WebhookResponseService(ILogger<WebhookResponseService> logger,
            UserContext userContext,
            IProfiseeEntitiesClient profiseeEntitiesClient,
            IProfiseeRecordsClient profiseeRecordsClient)
        {
            this.logger = logger;
            this.userContext = userContext;
            this.profiseeEntitiesClient = profiseeEntitiesClient;
            this.profiseeRecordsClient = profiseeRecordsClient;
        }

        /// <summary>
        /// Sample method that handles the data passed from the Profisee service to the webhook.
        /// Here you could handle the call synchronously by executing whatever logic with the data
        /// provided that you would want, asynchronously handle it by offloading it to a message
        /// queue or some other processing service, and/or make a call to the Profisee API to further
        /// fasciliate whatever logic you want to execute.
        /// </summary>
        public async Task<WebhookResponseDto> ProcessRequest(Dictionary<string, object> data)
        {
            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Arg Data:");
            foreach (var kvp in data)
            {
                this.logger.LogInformation($"Key: {kvp.Key} - Value: {kvp.Value}");
            }

            // The workflow optionally allows for a response in the form of an object with the properties
            // ProcessingStatus and ResponsePayload. How these properties are used is up to the workflow designer.
            var result = new WebhookResponseDto
            {
                ProcessingStatus = 0,
                ResponsePayload = data
            };

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return result;
        }

        public async Task<WebhookResponseDto> ProcessRequest(WebhookRequestDto dto)
        {
            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Arg Data:");

            this.logger.LogInformation($"EntityId: {dto.EntityId}");
            this.logger.LogInformation($"Code: {dto.Code}");

            var result = new WebhookResponseDto
            {
                ProcessingStatus = 0
            };

            result.ResponsePayload.Add(nameof(dto.Code), dto.Code);

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return result;
        }

        public async Task ProcessRequest(SubscriberPayloadDto dto)
        {
            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Arg Data:");

            var payloadString = JsonConvert.SerializeObject(dto, Formatting.Indented);

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - {payloadString}");

            this.logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");
        }

        public async Task<WebhookResponseDto> UpdateDescriptionFromRequest(WebhookRequestDto dto)
        {
            var response = new WebhookResponseDto();

            this.profiseeEntitiesClient.SetAuthorizationHeader(this.userContext.SecurityToken.RawData);
            this.profiseeRecordsClient.SetAuthorizationHeader(this.userContext.SecurityToken.RawData);

            var getEntityResponse = await this.profiseeEntitiesClient.GetEntityAsync(dto.EntityId);

            if (!getEntityResponse.Success)
            {
                var errorDto = getErrorFromProfiseeResponse(getEntityResponse);

                response.ResponsePayload.Add("Error", errorDto);
                response.ProcessingStatus = -1;

                return response;
            }

            var description = $"{getEntityResponse.Entity.Identifier.Name} - {dto.Code} was updated by the Webhook using the Profisee Rest API at {DateTime.Now}";
            var attributeNameValuePair = new Dictionary<string, object>
            {
                {"Description", description}
            };

            var updateRecordResponse = await this.profiseeRecordsClient.UpdateRecordAsync(dto.EntityId, dto.Code, attributeNameValuePair);

            if (!updateRecordResponse.Success)
            {
                var errorDto = getErrorFromProfiseeResponse(updateRecordResponse);

                response.ResponsePayload.Add("Error", errorDto);
                response.ProcessingStatus = -1;
            }

            return response;
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
    }
}