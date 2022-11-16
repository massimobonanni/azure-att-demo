targetScope = 'subscription'

@description('The location wher you want to create the resources.')
param location string = deployment().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@minLength(3)
param environmentName string = 'nsgDemo${uniqueString(subscription().id)}'

@description('The base ip address for the network address space.')
param vnetIpAddress string = '10.0.0.0'

@description('The CIDR mask number for the VNet.')
param cidrMask int = 24

@description('Admin username for vms.')
param adminUsername string

@description('Admin password for vms.')
@minLength(12)
@secure()
param adminPassword string

var resourceGroupName='${environmentName}-rg'

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
    vnetIpAddress: vnetIpAddress
    cidrMask: cidrMask
    adminUsername: adminUsername
    adminPassword: adminPassword
  }
}
