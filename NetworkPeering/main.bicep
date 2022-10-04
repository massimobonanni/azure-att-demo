targetScope = 'subscription'

@description('The location where you want to create the first network and vm.')
param location1 string=deployment().location

@description('The location where you want to create the second network and vm.')
param location2 string

@description('Admin username for vms.')
param adminUsername string

@description('Admin password for vms.')
@minLength(12)
@secure()
param adminPassword string

var ipAddress1 ='10.0.0.0'
var ipAddress2 ='10.0.1.0'
var cidrMask=24

resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: 'NetworkPeering-rg'
  location: location1
}

module net1Resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'net1Resources'
  params: {
    location: location1
    vnetIpAddress:ipAddress1
    cidrMask:cidrMask
    adminUsername:adminUsername
    adminPassword:adminPassword
  }
}

module net2Resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'net2Resources'
  params: {
    location: location2
    vnetIpAddress:ipAddress2
    cidrMask:cidrMask
    adminUsername:adminUsername
    adminPassword:adminPassword
  }
}
