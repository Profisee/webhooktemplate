using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebhookTemplate.AzureFunction.Payloads;

public class WorkflowWebhookResponse
{
    [JsonProperty(Required = Required.Default)]
    public int ProcessingStatus { get; set; }
    [JsonProperty(Required = Required.Default)]
    public Dictionary<string, object> ResponsePayload { get; set; }
}
