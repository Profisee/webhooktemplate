//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

namespace Profisee.WebhookTemplate.Clients
{
    internal interface IBaseProfiseeClient
    {
        void SetAuthorizationHeader(string value);
    }
}