# Adventure Works LT SQL Database demo IaC

This project contains the bicep templates you can use to create the environment to host Event Grid Demo.

To create the resource group and to deploy the resources you need for the project, simply run the following command:

```
az deployment sub create --location <your region> --template-file main.bicep
```

where 
- `<your region>` is the location where you want to create the deployment


You can also set these parameters:

- `location` : the location you want to deploy (by default the location is the same of your deployment)
- `resourceGroupName` : the name of the resource group contains the resources. The default value is `SQLServerDemo-rg`.
- `environmentName` : the prefix used to vreate the resources in the solution. The default value is `sqlDemo`.
- `sqlAdminUser` : the username for admin user in the SQL Server created by the template.
- `sqlAdminPwd` : the password for the admin user in the SQL Server created by the template. 

