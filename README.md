# Profisee .NET Webhook Templates

This repository contains Profisee Webhook templates and describes how to utilize them. Webhooks work with Profisee's Workflow or Real-Time Event Processing service to send customized payloads to a webhook endpoint and have responses sent back that can be utilized by the Workflow Activity. Events can then be configured to send information related to a specific entity.

## Additional Files

[Simple model and data archive](https://github.com/Profisee/webhooktemplate/tree/story/132784/AdditionalFiles) to restore and test against, along with a sample activity that can be imported via FastApp Studio.

## Documents

[Contains](https://github.com/Profisee/webhooktemplate/tree/story/132784/Documents) documentation on each project.

## WebhookTemplate.AzureFunction

[Sample Azure Functions project](https://github.com/Profisee/webhooktemplate/tree/story/132784/WebhookTemplate.AzureFunction) containing code for setting up webhooks using Azure Functions.

## WebhookTemplate.WebApp

[Sample web app project](https://github.com/Profisee/webhooktemplate/tree/story/128376/WebhookTemplate.WebApp) that can be compiled and deployed to use as webhook for Profisee Service webhook activities and events.

## NodeTemplate

[Sample Node.js project](https://github.com/Profisee/webhooktemplate/tree/main/NodeTemplate) containing code for setting up webhooks using Node.js.

## Webhook Activity Library

[Workflow Activity project](https://github.com/Profisee/webhooktemplate/tree/story/128376/WebhookTemplateActivityLibrary) that contains the uncompiled SampleWebhookActivity.wf workflow file found in AdditionalFiles. This can be used to see how webhook activities are configured.
