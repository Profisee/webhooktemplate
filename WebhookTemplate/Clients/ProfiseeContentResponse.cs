//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

namespace Profisee.WebhookTemplate.Clients
{
    internal class ProfiseeContentResponse : ProfiseeResponse
    {
        public string Content { get; set; }

        public bool HasContent => !string.IsNullOrWhiteSpace(this.Content);
    }
}