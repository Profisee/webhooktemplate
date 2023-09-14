using Profisee.WebhookTemplate.WebApp.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Requests
{
    public class WebhookActivityTypedRequestExample : IExamplesProvider<WebhookRequestDto>
    {
        public WebhookRequestDto GetExamples()
        {
            return new WebhookRequestDto
            {
                Code = "Profisee-01"
            };
        }
    }
}