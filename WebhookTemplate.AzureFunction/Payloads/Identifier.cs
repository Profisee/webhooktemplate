using System;

namespace WebhookTemplate.AzureFunction.Payloads;

public class Identifier
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
