# Network Peering Demo IaC

This project contains the bicep templates you can use to create the environment to host Network Peering Demo.

To create the resource group and to deploy the resources you need for the project, simply run the following command:

```
az deployment sub create --location <deploy region> --template-file main.bicep --parameters location1=<primary region> location2=<secondary region> adminUsername=<adminUser> adminPassword=<adminPassword>
```

where 
- `<deploy region>` is the location where you want to create the deployment
- `<primary region>` is the location where the first network will be created (default is the location of the deployment) 
- `<secondary region>` is the location where the second network will be created 
- `<adminUser>` the admin username for the VMs
- `<adminpassword>` the admin password for the
