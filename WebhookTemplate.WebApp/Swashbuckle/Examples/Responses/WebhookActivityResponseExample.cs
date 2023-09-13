using Profisee.WebhookTemplate.WebApp.Dtos;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

namespace Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Responses
{
    public class WebhookActivityResponseExample : IExamplesProvider<WebhookResponseDto>
    {
        public WebhookResponseDto GetExamples()
        {
            return new WebhookResponseDto
            {
                ProcessingStatus = 123,
                ResponsePayload = new Dictionary<string, object>
                {
                    { "Profisee" , "Now you can make it happen." },
                    { "Key", "Any value you want!" },
                    { "Data", "For your workflow."},
                    { "Salami", "A little, as a treat."}
                }
            };
        }
    }
}
