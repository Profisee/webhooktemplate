using Microsoft.AspNetCore.Routing;

namespace WebhookTemplate.WebApp.Mvc
{
    internal class CamelCaseRoutingConvention : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            var route = value.ToString();

            route = char.ToLowerInvariant(route[0]) + route.Substring(1);

            return route;
        }
    }
}
