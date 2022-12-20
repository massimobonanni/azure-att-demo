# Load balancer

This demo allows you to create a Standard Load Balancer with multiple VMs in the backend pool.


## Create the Demo
To create the demo, run the following command:

```bash
az deployment sub create --location <deployment location> --template-file main.bicep --parameters adminUsername=<admin username> adminPassword=<admin password> numberOfVMs=<number of VMs> demoName=<demo name> location=<resources location> vmSize=<VM size>
```

where
- `<deployment location>` : The location to store the deployment metadata
- `<admin username>` : The admin username for the VMs
- `<admin password>` : The admin password for the VMs
- `<number of VMs>` : The number of VMs in the backend pool. The default value is 2.
- `<demo name>` : The demo name is a string used by the template to create resource group name and resources name. The default value is 'LB'.
- `<location>` : The location in which you want to deploy the resource group and the resources. The default value is deployment location.
- `<VM size>` : The size of the VMs created by the template. The default value is `Standard_B1s`.

## Install IIS using AZ CLI
To setup the IIS on VMs and set a new default html page you can run the following command for each VMs created with the template:

```bash
az vm run-command invoke --command-id RunPowerShellScript --name <vmname> --resource-group <resource group name> --script installIIS.ps1
```

where
- `<vmname>` : Is the name of the VM;
- `<resource group name>` : the name of the resource group in which you deploy the VMs