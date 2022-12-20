targetScope = 'subscription'

@description('Admin username for the Virtual Machines.')
param adminUsername string

@description('Admin password for the Virtual Machines.')
@minLength(12)
@secure()
param adminPassword string

@description('Demo name is used to create the resource group name and resources names')
param demoName string = 'LB'

@description('Azure location in which you create the resources')
param location string= deployment().location

@description('Number of VMs to create in the Load Balancer backend pool')
param numberOfVMs int = 2

@description('Size of the VMs. The default value is Standard_B1s')
param vmSize string = 'Standard_B1s'

var resourceGroupName ='${demoName}Demo-rg'


resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: resourceGroupName
  location: location
}

module resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'resources'
  params: {
    location: location
    demoName:demoName
    adminUsername:adminUsername
    adminPassword:adminPassword
    numberOfVMs:numberOfVMs
    vmSize:vmSize
  }
}

module webServers 'webServersSetup.bicep' = {
  scope: resourceGroup
  name: 'webServers'
  params: {
    location: location
    demoName:demoName
    numberOfVMs:numberOfVMs
  }
  dependsOn:[
    resources
  ]
}

