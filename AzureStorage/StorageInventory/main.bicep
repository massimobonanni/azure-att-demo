targetScope = 'subscription'

@description('The location wher you want to create the resources.')
param location string=deployment().location


resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-01-01' = {
  name: 'StorageInventory-rg'
  location: location
}

module resources 'resources.bicep' = {
  scope: resourceGroup
  name: 'resources'
  params: {
    location: location
  }
}

