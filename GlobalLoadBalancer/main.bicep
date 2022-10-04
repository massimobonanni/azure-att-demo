targetScope = 'subscription'

param primaryLBLocation string
param secondaryLBLocation string
@allowed([
  'eastus2'
  'westus'
  'westeurope'
  'southeastasia'
  'centralus'
  'northeurope'
  'eastasia'
  'usgovvirginia'
])
param globalLBLocation string
param adminUsername string
@description('Password for the Virtual Machine.')
@minLength(12)
@secure()
param adminPassword string

var primaryRGName ='PrimaryLB-rg'
var secondaryRGName ='SecondaryLB-rg'
var globalRGName ='GlobalLB-rg'

resource primaryResourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: primaryRGName
  location: primaryLBLocation
}

resource secondaryResourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: secondaryRGName
  location: secondaryLBLocation
}

resource globalResourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: globalRGName
  location: globalLBLocation
}

module primaryRegion 'localLB.bicep' = {
  scope: primaryResourceGroup
  name: 'primaryRegion'
  params: {
    location: primaryLBLocation
    envSuffix:'Primary'
    adminUsername:adminUsername
    adminPassword:adminPassword
  }
}

module secondaryRegion 'localLB.bicep' = {
  scope: secondaryResourceGroup
  name: 'secondaryRegion'
  params: {
    location: primaryLBLocation
    envSuffix:'Secondary'
    adminUsername:adminUsername
    adminPassword:adminPassword
  }
}

module globalModule 'globalLB.bicep' = {
  scope: globalResourceGroup
  name: 'globalEnvironment'
  params: {
    location: globalLBLocation
  }
}
