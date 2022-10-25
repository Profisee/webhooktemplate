//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System.Net;

namespace Profisee.WebhookTemplate.WebApp.Clients
{
    internal class ProfiseeResponse
    {
        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}