# NSG Demo IaC

This project contains the bicep templates you can use to create the environment to host NSG Demo.

To create the resource group and to deploy the resources you need for the project, simply run the following command:

```
az deployment sub create --location <your region> --template-file main.bicep --parameters adminUsername=<adminUser> adminPassword=<adminPassword>
```

where 
- `<your region>` is the location where you want to create the deployment
- `<adminUser>` the admin username for the VMs
- `<adminpassword>` the admin password for the

You can also set these parameters:

- `location` : the location you want to deploy (by default the location is the same of your deployment)
- `environmentName` : the name of the environment. The template uses this value to create the resource group name and the resources names. The default value is a randome string starting with nsgDemo
- `vnetIpAddress`: the VNet address space. The default value is 10.0.0.0
- `cidrMask`: the CIDR mask for the vnet. The default value is /24

```
az deployment sub create --location <your region> --template-file main.bicep --parameters location=<location to deploy> environmentName=<envName> vnetIpAddress=<address space> cidrMask=<mask>
```


The demo is composed of:
- VNet : a virtual Network 
- 3 VMS: the VMs simulate a three tier application with FrontEnd, backEnd and DB
- NSG : the network security group contains the rules to avoid FrontEnd communicates to DB and BackEnd and DB cannot be invoked by Internet