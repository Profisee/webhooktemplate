//==============================================================================
// Copyright (c) Profisee Corporation. All Rights Reserved.
//==============================================================================

namespace Profisee.WebhookTemplate.WebApp.Clients
{
    internal interface IBaseProfiseeClient
    {
        void SetAuthorizationHeader(string value);
    }
}