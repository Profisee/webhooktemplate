
# Profisee Sample .NET Webhook Template

Use this sample .NET template if you want a quick outline of how to setup a webhook to work with Profisee's Workflow Service. A Profisee workflow can be configured to send a customized payload to a webhook endpoint where further logic can be implemented to process the incoming payload, and a response can be sent back and utilized by the Workflow Activity.

## Requirements

This application requires the .NET Framework 6 runtime and SDK. This sample template does not include information on running this application with a particular web server technology, and it is currently set up to run on IIS for Windows. See the *Program.cs* file to configure for different web server technologies.

## Setup

Navigate to the *appsettings.json* file, and set the following variables:

- LoggingConnectionString (Optional) - Provides MSSQL Database logging support. Create and logs to the table *WebhookTemplate.Log*.
- LoggingFilePath (Optional) - Provides an additional file output location for logging information. Regardless of what is provided here, logging information will be sent to the console and to a file local to the executable location called *Logs/Log.log*.
- ServiceUrl (Required) - Set this to the value of your Profisee Service Url. This URL can be found in Profisee FastApp Studio under **Manage Connections**. This will allow for incoming http requests and its Json Web Token (JWT) from the Profisee Workflow service to be authenticated.

Once the above has been completed, you should be able to build and run this sample template. For more specifics on the logic used in this sample, please refer to the included code comments, starting with the *Program.cs* and *Startup.cs* files.

## Additional Files

To test this template against the provided workflow, you must restore the model, archive, and workflow provided in the **Additional Files** folder. Configure the workflow's **Endpoint URL** value to the URL for the webhook. E.g, https://myurl.com/Profisee/v1/webhook/example1.

## Swagger/Swashbuckle

This sample includes a Swagger page for reference. Once this program is compiled and running, navigate to the URL defined in the *launchSettings.json* file and append "/index.html" to the URL to access the Swagger page. E.g., https://myurl.com/index.html.

## Content

Two endpoints are available in this application. They operate in the following manner:

- Example1 - This endpoint demonstrates receiving an untyped object in the form of a generic string-object dictionary. The backing service logs the content of the dictionary, and then returns a WebhookResponseDto object.
- Example2 - This endpoint demonstrates receiving a typed object in the form of WebhookRequestDto. This binds the content received in the http request body to the WebhookRequestDto object, and then outputs the code value. It's best practice to use typed objects when receiving incoming requests from the Profisee service, as it is more easy to build validation logic and error handling around typed objects than a generic dictionary representing an object.

## Additional Notes

Files to note:

- **Controllers/WebhookController.cs** - This is where the webhook endpoint is located.
- **Services/WebhookResponseService.cs** - This is where the the data for the webhook is processed.
- **Extensions/Authentication/AddJwtAuthenticationExtension.cs** - This is where the configuration for the JWT authentication is located.
- **Startup.cs** - This is where the core setup and configuration logic is located. Additional configuration is handled within the extensions folder.