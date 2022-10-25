using System.Collections.Generic;
using Swashbuckle.AspNetCore.Filters;

namespace Profisee.WebhookTemplate.WebApp.Swashbuckle.Examples.Requests
{
    public class WebhookActivityGenericRequestExample : IExamplesProvider<Dictionary<string, object>>
    {
        public Dictionary<string, object> GetExamples()
        {
            return new Dictionary<string, object>
            {
                { "Profisee" , "Now you can make it happen." },
                { "Key", "Any value you want!" },
                { "Data", "From your workflow." },
                { "Salami", "A little, as a treat."}
            };
        }
    }
}