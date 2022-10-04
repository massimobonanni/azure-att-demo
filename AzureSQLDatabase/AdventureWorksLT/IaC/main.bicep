targetScope = 'subscription'

@description('The name of the resource group that contains all the resources')
param resourceGroupName string = 'SQLServerDemo-rg'

@description('The location wher you want to create the resources.')
param location string=deployment().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@minLength(3)
param environmentName string = 'sqlDemo${uniqueString(subscription().id)}'

@description('Username for the admin in SQL Server')
param sqlAdminUser string

@description('Password for the admin in SQL Server')
@secure()
param sqlAdminPwd string

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'resources'
  params: {
    location: location
    environmentName: environmentName
    sqlAdminPwd: sqlAdminPwd
    sqlAdminUser: sqlAdminUser
  }
}

