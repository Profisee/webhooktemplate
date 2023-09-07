namespace WebhookTemplate.AzureFunction.Payloads;

public class WorkflowPayload
{
    public Identifier EntityObject { get; set; }
    public string MemberCode { get; set; }
    public string Description { get; set; }
}
