using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Profisee.WebhookTemplate.WebApp.Dtos;
using Profisee.WebhookTemplate.WebApp.Services;
using Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Requests;
using Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Responses;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;

namespace Profisee.WebhookTemplate.WebApp.Controllers
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

        // The following two endpoints are examples of what the Profisee service would call during execution of
        // a workflow activity. Note that through attribute tagging, this has been marked as a
        // Post call, and has also been marked as an endpoint that requires authorization. This means
        // that when this endpoint is called, it is expecting a JWT to be included in the authorization
        // header of the request. That JWT validation is handled by the ASP.NET Core authorization logic
        // and is configured in the Extensions/Authentication/AddJwtAuthenticationExtension.cs file.

        /// <summary>
        /// Webhook activity with generic request parameters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Endpoint for a webhook activity. This endpoint takes a generic string-object dictionary
        /// and responds with the WebhookResponseDto.
        /// </para>
        /// </remarks>
        [HttpPost("activity-generic")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(WebhookActivityGenericRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WebhookActivityResponseExample))]
        public async Task<IActionResult> ActivityGeneric([FromBody] Dictionary<string, object> body)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            var response = await webhookResponseService.ProcessRequest(body);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            // The Profisee service expects a successful http response status code (20x)
            // In this case, we'll return 200 OK
            return Ok(response);
        }

        /// <summary>
        /// Webhook activity with a typed request.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Endpoint for a webhook activity. This endpoint takes a WebhookRequestDto and responds with the WebhookResponseDto.
        /// For the developer of the workflow and/or webhook, the request object can be whatever they want. Creating an object
        /// to receive data from a webhook activity instead of a generic dictionary is the suggested approach to designing a
        /// webhook.
        /// </para>
        /// </remarks>
        [HttpPost("activity-typed")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(WebhookActivityTypedRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WebhookActivityResponseExample))]
        public async Task<IActionResult> ActivityTyped([FromBody] WebhookRequestDto dto)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            var response = await webhookResponseService.ProcessRequest(dto);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return Ok(response);
        }

        // The following endpoint is an example of what the Profisee service would call if there is a
        // configured subscriber. The attribute tagging matches the previous two endpoints.

        /// <summary>
        /// Event subscriber.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Endpoint for an event subscriber. This endpoint takes a SubscriberPayloadDto and does not return an
        /// object. The SubscriberPayloadDto matches what the Profisee Service sends when an event is sent.
        /// </para>
        /// </remarks>
        [HttpPost("subscriber")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(WebhookSubscriberRequestExample))]
        public async Task<IActionResult> Subscriber([FromBody] SubscriberPayloadDto dto)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            await webhookResponseService.ProcessRequest(dto);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return Ok();
        }

        [HttpPost("update-entity-description")]
        [Authorize]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WebhookResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(WebhookActivityTypedRequestExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(WebhookActivityResponseExample))]
        public async Task<IActionResult> UpdateEntityDescription([FromBody] WebhookRequestDto dto)
        {
            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Entry");

            var response = await webhookResponseService.UpdateDescriptionFromRequest(dto);

            logger.LogInformation($"{this.GetType().Name}.{MethodBase.GetCurrentMethod().Name} - Exit");

            return Ok(response);
        }
    }
}
