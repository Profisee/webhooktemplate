# Profisee Sample .NET Webhook Template using Azure Functions
Use this sample .NET template as a quick outline for setting up a webhook to work with Profisee's Workflow or Event Subscription service through Azure Functions. This is helpful when working with a Platform as a Service (PaaS) instance of Profisee. A Profisee workflow can be configured to send a customized payload to a webhook endpoint where further logic can be implemented to process the incoming payload, and a response can be sent back and utilized by the Workflow Activity. A Profisee event can be configured to send information about an event related to a specific entity.

## Requirements

- .NET Framework 6 runtime
- .NET Framework 6 SDK
- Active Azure Subscription

This sample template is configured for running the Azure Function and does not require a web server technology. This also assumes you are running the Profisee instance as a PaaS offering. There is also no use of Azure Logic apps for this template. If you want more information on deploying Azure Functions and Azure Logic Apps to work with Profisee's Webhook infrastructure, please view the training video on "Advanced Webhooks" [online guide](https://support.profisee.com/lms/courseinfo?id=00u00000000004T00aM)

## Optional

- Visual Studio
- Visual Studio Windows Workflow Foundation component
- Profisee SDK

Included in this project is a webhook activity library with a sample webhook activity. If you want to compile it and register it with the workflow system, you must have Visual Studio installed with the Windows Workflow Foundation component. You must also have the Profisee SDK. To read more about installing the Profisee SDK, visit our [online guide](https://support.profisee.com/wikis/2022_r1_support/profisee_sdk_installation). If you do not wish to do this, you can import the SimpleWebhookActivity.wf workflow file definition.

## Webhook Setup

Compile the the *WebhookTemplate* project and publish it to the Azure Portal, from here you can create a new Azure Function App, or you can publish it to one you already created. Once that is created, you can then go to the Azure Portals and on your Function App's configuration settings, and create new application settings for:

- LoggingFilePath (Optional) - Provides an additional file output location for logging information. Regardless of what is provided here, logging information will be sent to the console and to a file local to the executable location called *Logs/Log.log*.
- ServiceUrl (Required) - Set this to the value of your Profisee Service Url. This URL can be found in your Kubernetes Service under the configuration map "profisee-settings". The url will be named "ProfiseeExternalDNSUrl.

Please note that these values also exist under the "local.settings.json" file in the Azure Function project. You can directly modify the ServiceUrl and LoggingFilePath there to point to a local instance of profisee when locally testing/debugging, and the function app will still use the global setting you set in your configuration settings.

## Workflow Setup

To test against the provided workflow, you must restore the model, archive, and workflow provided in the **Additional Files** folder. Configure the workflow's **Endpoint URL** value to the URL for the webhook. E.g, https://azurefunctionappURL.azurewebsites.net/api/WorkflowWebhookActivity. It can also be set to a local host directory for debugging purposes

## Event Subscriber Setup

1. In FastApp Studio, navigate to the **Real-Time Event Processing** tab.
2. Disable real-time event processing.
3. In the **Subscribers** page, ensure the Webhook Subscriber is under the **Registered Subscribers** list.
4. In the **subscriber-configuration** page, create a new configuration. Give it a name, ensure the subscriber is set to the webhook subscriber, configure the *run-as* user under the properties section, then set the url for your webhook subscriber endpoint. E.g, https://azurefunctionappURL.azurewebsites.net/api/Subscriber
5. Create an event. To do so, you must have a deployed model with an entity to associate your event with. A sample model and archive has been included in the *Additional Files* folder to restore if necessary. Ensure the event is enabled and the subscriber configuration created in the previous step is selected.
6. Enable real-time event processing.

## Azure Portal Setup

1.	In Azure Portal, create a function app (Azure services > Function App -> Create)
2.	Once the function is created, go to Configuration-> Application Settings->New application setting:
    - Name: ServiceUrl 
    - Value: external DNS URL (ex: https://profiseenew.corp.profisee.com/profisee/). 
        *Note: The App URL should have “/profisee/” at the end
3.	Microsoft Visual Studio (MVS): 
    - Clone/Download the template: Profisee/webhooktemplate (github.com) and publish the Azure Function App:
        1. WebhookTemplate.AzureFunction -> Publish -> Target: Azure -> Specific target: Azure Function App (Windows) -> Story128376LogicApp: Story128376FunctionApp -> Publish
        2. After clicking the 'Publish' button, wait for the ‘Function app to be ready…’
    - Go to Function App in Azure Portal and verify that 2 Functions presented in the Overview section: 
      1. **Subscriber** 
      2. **WorkflowWebhookActivity** 


## Running the functions

1. In postman, create the necessary collection for the instance of Profisee you are running
    - Access Token URL with come from your platform’s AKS cluster’s configuration, the ProfiseeExternalDNSUrl

2. Create Http Post calls for the different endpoints
    - This can be grabbed through the Azure Portal by going into each function, going to code+test and then grabbing the “function Url”

## Endpoints

There are two endpoints available in this application.

Go to Azure Portal -> Function App -> Pick your Function App ->Pick function in the Name section -> Click on ‘Get Function URL’ at the top command Bar.

They operate in the following manner:
   - **api/WorkflowWebhookActivity** - This endpoint will receive a workflow payload, load a selected entity and record, update the description, and push the update back to the profisee service. It grabs the Authorization from the header, validates the JWT, binds the content received in the HTTP request body to a Workflow Payload object, and will push back the update request to the profisee service. This should be a good foundation for building more complex functionality.
    
   - **api/Subscriber** - This endpoint will be used with the ‘Real Time Event Processing in the Profisee Fast App Studio. This endpoint receives a Subscriber Payload and updates the description of the selected record on the entity. It's similar to the workflow function, only working as a subscriber.

## Additional Notes

Files to note:

- **WebhookTemplate.AzureFunction\Functions\WorkflowWebhookActivity.cs** - This is where the UpdateEntityDescription endpoint is located.
- **WebhookTemplate.AzureFunction\Functions\Subscribers.cs** - This is where the Subscribers endpoint is located.
- **WebhookTemplate.AzureFunction\local.settings.json** - This is where the ServiceUrl can be inputted for local testing.

### Invalid JWT Sent to Webhook

This article describes what is occuring when you get an invalid JWT error, and a solution to resolve it.

#### The Invalid JWT Error

When creating the JWT that is sent to the webhook, we include values for 'not valid before' (nbf) and 'expiration time' (exp). 
These values are sent over as a 'date-time' given down to the second. 
Those values seem to be based on the current time, with 'not valid before' being the current time and 'expiration time' being current time plus x minutes. 
But because these times are generated in seconds, there can be cases in which the call the webhook occurs before the JWT is valid. 

In the other cases, Azure services are used and time is maintained in milliseconds. 
As a result, when the webhook receives the call and tries to validate the JWT, it's is not (yet) valid.
The calls come in right before the clock will tick over to the next second, so technically these calls are not valid. 

Modifying the clock skew will resolve this error. MS JWT has a built in 5 minute clock skew, and modifying this value will allow it the ability to add offset the already built in 5 minutes.

#### About this solution template

When authenticating, the code will be responsible for implementing clock skew if the nbf claim is required for authorization. If this error ocurs, it can be resolved
by changing the clock skew in the code.

##### Steps

1. [Here](https://github.com/Profisee/webhooktemplate/blob/story/132784/WebhookTemplate.AzureFunction/Functions/WorkflowWebhookActivity.cs) you can change the clock skew.
2. Go to the getTokenValidationParameters function and scroll to the paramater variable.
3. Change the ClockSkew TimeSpan from 5 minutes to desired value to resolve JWT Invalidation issue
