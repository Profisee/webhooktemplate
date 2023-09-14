namespace Profisee.WebhookTemplate.AzureFunction.Clients
{
    public class ProfiseeContentResponse : ProfiseeResponse
    {
        public string Content { get; set; }

        public bool HasContent => !string.IsNullOrWhiteSpace(this.Content);
    }
}