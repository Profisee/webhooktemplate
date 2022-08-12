//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

using System;

namespace Profisee.WebhookTemplate.Clients.Dtos
{
    internal  class IdentifierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int InternalId { get; set; }
        public bool IsReferenceValid { get; set; }
    }
}
