using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Profisee.WebhookTemplate.Common.Dtos;
using Profisee.WebhookTemplate.WebApp.Services;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebhookTemplate.AzureFunction.Functions
{
    public class MyFunction
    {
        private readonly IWebhookResponseService webhookResponseService;

        public MyFunction(IWebhookResponseService webhookResponseService)
        {
            this.webhookResponseService = webhookResponseService;
        }

        [FunctionName("MyFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
             ILogger log)
        {
            using var reader = new StreamReader(request.Body);
            var json = await reader.ReadToEndAsync();
            var dto = JsonSerializer.Deserialize<WebhookRequestDto>(json);

            var result = await webhookResponseService.UpdateDescriptionFromRequest(dto);

            return new OkObjectResult(result);
        }
    }
}