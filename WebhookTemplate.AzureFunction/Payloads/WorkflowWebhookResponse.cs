using System.Collections.Generic;

namespace WebhookTemplate.AzureFunction.Payloads;

public class WorkflowWebhookResponse
{
    public int ProcessingStatus { get; set; }

    public Dictionary<string, object> ResponsePayload { get; set; }
}
