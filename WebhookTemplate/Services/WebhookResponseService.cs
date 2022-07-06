//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Profisee.WebhookTemplate.Contexts;
using Profisee.WebhookTemplate.Dtos;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.Services
{
    /// <summary>
    /// Sample service used to handle incoming webhook requests.
    /// </summary>
    public class WebhookResponseService : IWebhookResponseService
    {
        private readonly ILogger<IWebhookResponseService> logger;
        private readonly UserContext userContext;

        public WebhookResponseService(ILogger<WebhookResponseService> logger, UserContext userContext)
        {
            this.logger = logger;
            this.userContext = userContext;
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
    }
}