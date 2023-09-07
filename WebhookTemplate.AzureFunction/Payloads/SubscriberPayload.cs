namespace WebhookTemplate.AzureFunction.Payloads;

public class SubscriberPayload
{
    public Identifier EntityObject { get; set; }
    public string MemberCode { get; set; }
    public int Transaction { get; set; }
    public string UserName { get; set; }
    public string EventName { get; set; }
}
