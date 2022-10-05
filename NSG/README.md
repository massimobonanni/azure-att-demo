# Event Grid Demo IaC

This project contains the bicep templates you can use to create the environment to host Event Grid Demo.

To create the resource group and to deploy the resources you need for the project, simply run the following command:

```
az deployment sub create --location <your region> --template-file main.bicep
```

where 
- `<your region>` is the location where you want to create the deployment


You can also set these parameters:

- `location` : the location you want to deploy (by default the location is the same of your deployment)

```
az deployment sub create --location <your region> --template-file main.bicep --parameters location=<location to deploy>
```

The demo is composed of:
- Storage Account: this storage emits event and the events are redirected to event grid viewer throught the event grid topic
- Event Grid Topic : captures the event from the storage and rediect to event grid viewer using the subscription
- Event Grid Subscription : connects the topic to the event grid viewer
- App Service: is the handler for the subscription that routes the event from storage