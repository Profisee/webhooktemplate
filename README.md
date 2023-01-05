
# Profisee Sample .NET Webhook Template

Use this sample .NET template as a quick outline for setting up a webhook to work with Profisee's Workflow or Event Subscription service. A Profisee workflow can be configured to send a customized payload to a webhook endpoint where further logic can be implemented to process the incoming payload, and a response can be sent back and utilized by the Workflow Activity. A Profisee event can be configured to send information about an event related to a specific entity.

## Requirements

- .NET Framework 6 runtime
- .NET Framework 6 SDK

This sample template does not include information on running this application with a particular web server technology, and it is currently set up to run on IIS for Windows. See the *Program.cs* file to configure for different web server technologies.

## Optional

- Visual Studio
- Visual Studio Windows Workflow Foundation component
- Profisee SDK

Included in this project is a webhook activity library with a sample webhook activity. If you want to compile it and register it with the workflow system, you must have Visual Studio installed with the Windows Workflow Foundation component. You must also have the Profisee SDK. To read more about installing the Profisee SDK, visit our [online guide](https://support.profisee.com/wikis/2022_r1_support/profisee_sdk_installation). If you do not wish to do this, you can import the SimpleWebhookActivity.wf workflow file definition.

## Content

- Additional Files - A simple model and data archive to restore and test against, along with a sample activity that can be imported via FastApp Studio.
- Webhook Template - A sample webhook project that can be compiled and deployed to use as webhook for Profisee Service webhook activities and events.
- Webhook Activity Library - A Workflow Activity project that contains the uncompiled SampleWebhookActivity.wf workflow file found in AdditionalFiles. This can be used to see how webhook activities are configured.

## Webhook Setup

Compile the the *WebhookTemplate* project and deploy it to the web host service of your choice, then navigate to the *appsettings.json* file and set the following variables:

- LoggingConnectionString (Optional) - Provides MSSQL Database logging support. Create and logs to the table *WebhookTemplate.Log*.
- LoggingFilePath (Optional) - Provides an additional file output location for logging information. Regardless of what is provided here, logging information will be sent to the console and to a file local to the executable location called *Logs/Log.log*.
- ServiceUrl (Required) - Set this to the value of your Profisee Service Url. This URL can be found in Profisee FastApp Studio under **Manage Connections**. This will allow for incoming http requests and its Json Web Token (JWT) from the Profisee Workflow service to be authenticated.

Once the above has been completed, the Profisee Service is able to send messages to the webhook template. For more specifics on the logic used in this sample, please refer to the included code comments, starting with the *Program.cs* and *Startup.cs* files.

## Workflow Setup

To test against the provided workflow, you must restore the model, archive, and workflow provided in the **Additional Files** folder. Configure the workflow's **Endpoint URL** value to the URL for the webhook. E.g, https://myurl.com/profisee/v1/webhook/activity-typed.

## Event Subscriber Setup

1. In FastApp Studio, navigate to the **Real-Time Event Processing** tab.
2. Disable real-time event processing.
3. In the **Subscribers** page, ensure the Webhook Subscriber is under the **Registered Subscribers** list.
4. In the **subscriber-configuration** page, create a new configuration. Give it a name, ensure the subscriber is set to the webhook subscriber, configure the *run-as* user under the properties section, then set the url for your webhook subscriber endpoint. E.g, https://myurl.com/profisee/v1/webhook/subscriber.
5. Create an event. To do so, you must have a deployed model with an entity to associate your event with. A sample model and archive has been included in the *Additional Files* folder to restore if necessary. Ensure the event is enabled and the subscriber configuration created in the previous step is selected.
6. Enable real-time event processing.

## Swagger/Swashbuckle

This sample includes a Swagger page for reference. Once the webhook template is running, navigate to the URL defined in the *launchSettings.json* file and append "/index.html" to the URL to access the Swagger page. E.g., https://myurl.com/index.html.

The Swagger page documents the endpoints for the webhook, including information about the request and response schemas. These endpoints cannot be tested without providing a Profisee generated JWT within the *Authorize* button located above the endpoint list. A Profisee JWT can be captured in the WebhookTemplate logs when the Profisee Service makes a request.

## Endpoints

Three endpoints are available in this application. They operate in the following manner:

- "profisee/v1/webhook/activity-generic" - This endpoint demonstrates receiving an untyped object in the form of a generic string-object dictionary. The backing service logs the content of the dictionary, and then returns a WebhookResponseDto object.
- "profisee/v1/webhook/activity-typed" - This endpoint demonstrates receiving a typed object in the form of WebhookRequestDto. This binds the content received in the http request body to the WebhookRequestDto object, and then outputs the code value. It is best practice to use typed objects when receiving incoming requests from the Profisee service, as it is easier to build validation logic and error handling around typed objects than a generic dictionary representing an object.
- "profisee/v1/webhook/subscriber" - This endpoint is to be used with eventing. This endpoint receives a SubscriberPayloadDto containing information about the event that occured.

## Handling Response Objects

### Prior to 2023.R1

For the Webhook Workflow Activity, any [nonprimitive](https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=net-6.0) objects returned as a part of the ResponsePayload come back either as a JArray (any array or list) or a JObject (any complex object or dictionary). If the ResponsePayload contains any of these objects and the workflow reaches a Contribution or Approval Task Activity, the workflow will experience serialization errors. To prevent this, these objects will have to be converted using an Assign Activity by calling [ToObject](https://www.newtonsoft.com/json/help/html/ToObjectType.htm) on them back to the ResponsePayload.

### After 2023.R1

Starting in 2023.R1, these JArrays and JObjects are converted to List&lt;object&gt; or Dictionary<string, object> respectively by the Webhook Activity. Further conversion may need to be done via casting for further use in the workflow. However, there is no risk of the serialization errors from pervious releases.

### Further Recommendations

It is recommended that only primitives, List&lt;object&gt;, and Dictionary<string, object> are returned as a part of the ResponsePayload with the objects within the list and dictionaries also limited to primitives.

For example: 

**Good**
```
var responsePayload = new Dictionary<string, object> {
    { "approvers", new List<string> { "user1", "user2" } },
    { "recordCount", 5 },
    { "myDictionary", new Dictionary<string, string> {
        { "key1", "value1" },
        { "key2", "value2" }
    } }
};
```

**Bad**
```
var responsePayload = new Dictionary<string, object> {
    { "approvers", new List<string> { new User { Name = "Jimmy", Id = 1 } } },
    { "recordCount", new RecordData { Count = 5 } },
    { "myDictionary", new Dictionary<string, string> {
        { "key1", new ValueHolder { Value = "value1" } },
        { "key2", new ValueHolder { Value = "value2" } }
    } }
};
```

## Additional Notes

Files to note:

- **Controllers/WebhookController.cs** - This is where the webhook endpoints is located.
- **Services/WebhookResponseService.cs** - This is where incoming messages for the webhook is processed.
- **Extensions/Authentication/AddJwtAuthenticationExtension.cs** - This is where the configuration for authentication is located.
- **Startup.cs** - This is where the core setup and configuration logic is located. Additional configuration is handled within the extensions folder.
