@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@maxLength(16)
@minLength(3)
param environmentName string = 'si${uniqueString(subscription().id, resourceGroup().name)}'

var storageAccountName = toLower('${environmentName}dstore')
var functionAppStorageAccountName = toLower('${environmentName}appstore')
var funcHostingPlanName = toLower('${environmentName}-plan')
var functionAppName = toLower('${environmentName}-func')
var applicationInsightsName = toLower('${environmentName}-ai')

//-------------------------------------------------------------
// Storage Account
//-------------------------------------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
}

resource documentContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${storageAccount.name}/default/documents'
  properties: {
    publicAccess: 'Blob'
    metadata: {}
  }
}

resource inventoryContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2019-06-01' = {
  name: '${storageAccount.name}/default/inventory'
  properties: {
    publicAccess: 'None'
    metadata: {}
  }
}

resource inventoryRules 'Microsoft.Storage/storageAccounts/inventoryPolicies@2021-04-01' = {
  name: 'default'
  parent: storageAccount
  properties: {
    policy: {
      enabled: true
      type: 'Inventory'
      rules: [
          {
              destination: 'inventory'
              enabled: true
              name: 'AllBlobsInDocuments'
              definition: {
                  format: 'Csv'
                  schedule: 'Daily'
                  objectType: 'Blob'
                  schemaFields: [
                      'Name'
                      'Creation-Time'
                      'Last-Modified'
                      'Content-Length'
                      'Content-MD5'
                      'BlobType'
                      'AccessTier'
                      'AccessTierChangeTime'
                      'Metadata'
                      'LastAccessTime'
                  ]
                  filters: {
                      blobTypes: [
                          'blockBlob'
                      ]
                      prefixMatch: [
                        'documents/'
                      ]
                  }
              }
          }
      ]
    }
  }
}

resource storageRules 'Microsoft.Storage/storageAccounts/managementPolicies@2021-09-01' = {
  name: 'default'
  parent: storageAccount
  properties: {
    policy: {
      rules: [
        {
          enabled: true
          name: 'DocumentsRule'
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                tierToCool: {
                  daysAfterModificationGreaterThan: 5
                }
                delete: {
                  daysAfterModificationGreaterThan: 30
                }
              }
            }
            filters: {
              blobTypes: [
                'blockBlob'
              ]
              prefixMatch: [
                'documents'
              ]
            }
          }
        }
        {
          enabled: true
          name: 'InventoryRule'
          type: 'Lifecycle'
          definition: {
            actions: {
              baseBlob: {
                tierToCool: {
                  daysAfterModificationGreaterThan: 1
                }
                delete: {
                  daysAfterModificationGreaterThan: 60
                }
              }
            }
            filters: {
              blobTypes: [
                'blockBlob'
              ]
              prefixMatch: [
                'inventory'
              ]
            }
          }
        }
      ]
    }
  }
}


//-------------------------------------------------------------
// Function App
//-------------------------------------------------------------
resource functionAppStorageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: functionAppStorageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

resource funcHostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: funcHostingPlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: funcHostingPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionAppStorageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${functionAppStorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionAppStorageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${functionAppStorageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '~10'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'BlobsGeneratorConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'BlobsGeneratorContainerName'
          value: 'documents'
        }
        {
          name: 'BlobsGeneratorMaxNumberOfBlobs'
          value: '500'
        }
        {
          name: 'BlobsGeneratorTimer'
          value: '0 * * * *'
        }
      ]
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}
