# Invalid JWT Sent to Webhook

This article describes what is occuring when you get an invalid JWT error, and a solution to resolve it.

## The Invalid JWT Error

When creating the JWT that is sent to the webhook, we include values for 'not valid before' (nbf) and 'expiration time' (exp). 
These values are sent over as a 'date-time' given down to the second. 
Those values seem to be based on the current time, with 'not valid before' being the current time and 'expiration time' being current time plus x minutes. 
But because these times are generated in seconds, there can be cases in which the call the webhook occurs before the JWT is valid. 

In the other cases, Azure services are used and time is maintained in milliseconds. 
As a result, when the webhook receives the call and tries to validate the JWT, it's is not (yet) valid.
The calls come in right before the clock will tick over to the next second, so technically these calls are not valid. 

Modifying the clock skew will resolve this error. MS JWT has a built in 5 minute clock skew, and modifying this value will allow it the ability to add offset the already built in 5 minutes.

## About this solution template

When authenticating, the code will be responsible for implementing clock skew if the nbf claim is required for authorization. If this error ocurs, it can be resolved
by changing the clock skew in the code.

### Steps

1. [Here](https://github.com/Profisee/webhooktemplate/blob/story/132784/WebhookTemplate.AzureFunction/Functions/WorkflowUpdateEntityDescription.cs) you can change the clock skew.
2. Go to the getTokenValidationParameters function and scroll to the paramater variable.
3. Change the ClockSkew TimeSpan from 5 minutes to desired value to resolve JWT Invalidation issue
