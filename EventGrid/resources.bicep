@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

var storageAccountName = toLower('${uniqueString(resourceGroup().id)}store')
var appServicePlanName = toLower('eventbrowser-${uniqueString(resourceGroup().id)}-plan')
var appServiceName = toLower('eventbrowser-${uniqueString(resourceGroup().id)}-app')
var storageTopicName = toLower('${uniqueString(resourceGroup().id)}-topic')
var eventViewerSubName = toLower('${uniqueString(resourceGroup().id)}-subscription')

var viewerRepoUrl = 'https://github.com/azure-samples/azure-event-grid-viewer.git'

//-------------------------------------------------------------
// Storage Account
//-------------------------------------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
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

resource documentContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
  name: '${storageAccount.name}/default/documents'
  properties: {
    publicAccess: 'Blob'
    metadata: {}
  }
}

//-------------------------------------------------------------
// App service (event browser)
//-------------------------------------------------------------
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
  properties: {}
  kind: 'app'
}

resource appService 'Microsoft.Web/sites@2022-03-01' = {
  name: appServiceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    hostNameSslStates: [
      {
        hostType: 'Standard'
        sslState: 'Disabled'
        name: '${appServiceName}.azurewebsites.net'
      }
      {
        hostType: 'Standard'
        sslState: 'Disabled'
        name: '${appServiceName}.scm.azurewebsites.net'
      }
    ]
    siteConfig: {
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
    httpsOnly: true
  }
}

resource appServiceDeploy 'Microsoft.Web/sites/sourcecontrols@2022-03-01' = {
  parent: appService
  name: 'web'
  properties: {
    repoUrl: viewerRepoUrl
    branch: 'main'
    isManualIntegration: true
  }
}

//-------------------------------------------------------------
// Storage Topic
//-------------------------------------------------------------
resource storageTopic 'Microsoft.EventGrid/systemTopics@2022-06-15' = {
  name: storageTopicName
  location: location
  properties: {
    source: storageAccount.id
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

//-------------------------------------------------------------
// Event Viewer Subscription
//-------------------------------------------------------------
resource eventViewerSubscription 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2022-06-15' = {
  name: eventViewerSubName
  parent:storageTopic
  dependsOn: [
    appServiceDeploy
  ]
  properties: {
    destination: {
      endpointType: 'WebHook'
      properties: {
        maxEventsPerBatch: 1
        preferredBatchSizeInKilobytes: 64
        endpointUrl: 'https://${appService.properties.defaultHostName}/api/updates'
      }
    }
    filter: {
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        'Microsoft.Storage.BlobDeleted'
        'Microsoft.Storage.DirectoryRenamed'
        'Microsoft.Storage.BlobRenamed'
        'Microsoft.Storage.DirectoryDeleted'
        'Microsoft.Storage.DirectoryCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
  }
}
