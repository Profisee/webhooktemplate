using Profisee.WebhookTemplate.WebApp.Dtos;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Requests
{
    public class WebhookSubscriberRequestExample : IExamplesProvider<SubscriberPayloadDto>
    {
        public SubscriberPayloadDto GetExamples()
        {
            return new SubscriberPayloadDto
            {
                EntityObject = new EntityObjectDto
                {
                    Id = new Guid("00000000-0000-4000-0000-000000000001"),
                    Name = "Product",
                },
                MemberCode = "1",
                Transaction = 1000,
                UserName = "domain/userName",
                EventName = "Event Name",
            };
        }
    }
}