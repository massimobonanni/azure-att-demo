# NSG Demo IaC

This project contains the bicep templates you can use to create the environment to host NSG Demo.

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
``

The demo is composed of:
- VNet : a virtual Network 
- 3 VMS: the VMs simulate a three tier application with FrontEnd, backEnd and DB
- NSG : the network security group contains the rules to avoid FrontEnd communicates to DB and BackEnd and DB cannot be invoked by Internet