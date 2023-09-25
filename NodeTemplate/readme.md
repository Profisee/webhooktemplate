# Profisee Sample Node.js Webhook Template

Use this sample Node.js template as a quick outline for setting up a webhook to work with Profisee's Workflow or Event Subscription service. A Profisee workflow can be configured to send a customized payload to a webhook endpoint where further logic can be implemented to process the incoming payload, and a response can be sent back and utilized by the Workflow Activity. A Profisee event can be configured to send information about an event related to a specific entity.

## Requirements

- Node.js (this template was tested with v18.14.1)

This sample template does not include information on running this application with a particular web server technology, and it is currently set up to run on localhost with port 3000. The port number can be changed in the _config.js_ file.

Profisee requires that webhook endpoints are HTTPS. However, for simplicity, this example just runs a development server on localhost without HTTPS. This template does not include information on how to run this server on a production server with HTTPS, but for testing purposes you can use a tool like [ngrok](https://ngrok.com/) to expose your development server to the internet with HTTPS. After downloading and installing ngrok, start the development server for this app and then run the following commands:

```
ngrok http localhost:3000
```

ngrok will provide you with a forwarding address that you can use as your webhook endpoint, e.g, https://12345678.ngrok-free.app/webhooks/workflow-activity. Note that this endpoint will be publicly accessible.

## Optional

- Visual Studio
- Visual Studio Windows Workflow Foundation component
- Profisee SDK

Included in this repository is a webhook activity library with a sample webhook activity. If you want to compile it and register it with the workflow system, you must have Visual Studio installed with the Windows Workflow Foundation component. You must also have the Profisee SDK. To read more about installing the Profisee SDK, visit our [online guide](https://support.profisee.com/wikis/2022_r1_support/profisee_sdk_installation). If you do not wish to do this, you can import the SimpleWebhookActivity.wf workflow file definition.

## Webhook Setup

- SERVICE_URL (Required) - Open config.js and set the value of SERVICE_URL to your Profisee Service Url. This URL can be found in Profisee FastApp Studio under Manage Connections. This will allow for incoming http requests and its Json Web Token (JWT) from the Profisee Workflow service to be authenticated.
- PORT (Optional) - Open config.js and set the value of PORT to the port number on which you wish to run the development server. The default is 3000.

Navigate to the root directory of this project and run the following commands:

```
npm install
npm run start
```

This app will now be running on your machine at http://localhost:3000.

## Workflow Setup

To test against the provided workflow, you must restore the model, archive, and workflow provided in the **Additional Files** folder of this repository. Configure the workflow's **Endpoint URL** value to the URL for the webhook. E.g, https://my.serviceurl.com/Profisee/webhooks/workflow-activity. It can also be set to a local host directory for debugging purposes

## Event Subscriber Setup

1. In FastApp Studio, navigate to the **Real-Time Event Processing** tab.
2. Disable real-time event processing.
3. In the **Subscribers** page, ensure the Webhook Subscriber is under the **Registered Subscribers** list.
4. In the **subscriber-configuration** page, create a new configuration. Give it a name, ensure the subscriber is set to the webhook subscriber, configure the _run-as_ user under the properties section, then set the url for your webhook subscriber endpoint. E.g, https://my.serviceurl.com/Profisee/webhooks/subscriber
5. Create an event. To do so, you must have a deployed model with an entity to associate your event with. A sample model and archive has been included in the _Additional Files_ folder to restore if necessary. Ensure the event is enabled and the subscriber configuration created in the previous step is selected.
6. Enable real-time event processing.

## Endpoints

Two endpoints are available in this application. They operate in the following manner:

- **webhooks/workflow-activity** - This endpoint demonstrates receiving a JSON object payload from a workflow. The webhook simply logs the payload and then returns a response object.
- **webhooks/subscriber** - This endpoint is to be used with eventing. This endpoint receives a JSON payload containing information about the event that occured. It then uses the data in the payload to send a request to the Profisee REST API to update the description for the corresponding record. This endpoint currently expects a JSON object with the following shape:

```
{
  entityObject: {
    Id: string;
    Name: string;
  };
  membercode: string;
}
```

## Response Objects

Profisee expects a response from the webhook with the following shape, where _object_ is any JSON object:

```
{
  ProcessingStatus: number;
  ResponsePayload: object;
}
```
