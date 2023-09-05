using System.Net;

namespace Profisee.WebhookTemplate.Common.Clients
{
    public class ProfiseeResponse
    {
        public bool Success { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }
    }
}