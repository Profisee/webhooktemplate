//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Profisee.WebhookTemplate.Dtos;
using Profisee.WebhookTemplate.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("profisee/v{api-version:apiVersion}/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> logger;

        private readonly IWebhookResponseService webhookResponseService;

        public WebhookController(ILogger<WebhookController> logger,
            IWebhookResponseService webhookResponseService)
        {
            this.logger = logger;
            this.webhookResponseService = webhookResponseService;
        }

        /// <summary>
        /// A webhook endpoint. This is what the Profisee service will call during execution of
        /// the workflow activity. Note that through attribute tagging, this has been marked as a
        /// Post call, and has also been marked as an endpoint that requires authorization. This means
        /// that when this endpoint is called, it is expecting a JWT to be included in the authorization
        /// header of the request. That JWT validation is handled by the ASP.NET Core authorization logic
        /// and is configured in the Extensions/Authentication/AddJwtAuthenticationExtension.cs file.
        /// </summary>
        [HttpPost("example1")]
        [Authorize]
        public async Task<IActionResult> ExampleOne([FromBody] Dictionary<string, object> body)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            var response = await webhookResponseService.ProcessRequest(body);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            // The Profisee service expects a successful http response status code (20x)
            // In this case, we'll return 200 OK
            return Ok(response);
        }

        [HttpPost("example2")]
        [Authorize]
        public async Task<IActionResult> ExampleTwo([FromBody] WebhookRequestDto dto)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            var response = await webhookResponseService.ProcessRequest(dto);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return Ok(response);
        }
    }
}
