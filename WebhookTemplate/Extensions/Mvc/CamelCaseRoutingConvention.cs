//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using Microsoft.AspNetCore.Routing;

namespace Profisee.WebhookTemplate.WebApp.Extensions.Mvc
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
