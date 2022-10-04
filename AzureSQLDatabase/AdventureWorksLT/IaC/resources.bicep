@description('The location where you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@minLength(3)
param environmentName string = 'sqlDemo${uniqueString(resourceGroup().id)}'

@description('Username for the admin in SQL Server')
param sqlAdminUser string

@description('Password for the admin in SQL Server')
@secure()
param sqlAdminPwd string

//-------------------------------------------------------------
// WebSite
//-------------------------------------------------------------
var appName = toLower('${environmentName}-app')
var appPlanName = toLower('${environmentName}-plan')
var appInsightName = toLower('${environmentName}-appinsight')

resource appInsight 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

resource appService 'Microsoft.Web/sites@2021-01-01' = {
  name: appName
  location: location
  kind: 'app'
  properties: {
    enabled: true
    serverFarmId: appServicePlan.id
  }
  identity:{
     type: 'SystemAssigned'
  }
}

resource appSettings 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'appsettings'
  parent: appService
  properties: {
    APPINSIGHTS_INSTRUMENTATIONKEY: appInsight.properties.InstrumentationKey
  }
}

resource connectionStrings 'Microsoft.Web/sites/config@2022-03-01' = {
  name: 'connectionstrings'
  parent: appService
  properties: {
    DefaultConnection: {
      type: 'SQLAzure'
      value:  '@Microsoft.KeyVault(SecretUri=${sqlConnectionStringSecret.properties.secretUri})'
    }
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-01' = {
  name: appPlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
    size: 'F1'
    family: 'F'
    capacity: 0
  }
}

//-------------------------------------------------------------
// SQL Database
//-------------------------------------------------------------
var sqlServerName = toLower('${environmentName}-sqlsrv')
var sqlDBName = toLower('adventureworkslt')

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminUser
    administratorLoginPassword: sqlAdminPwd
    restrictOutboundNetworkAccess: 'Disabled'
    publicNetworkAccess: 'Enabled'
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  name: sqlDBName
  location: location
  parent: sqlServer
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  properties: {
    sampleName: 'AdventureWorksLT'
    autoPauseDelay: 60

  }
}

//-------------------------------------------------------------
// Key Vault
//-------------------------------------------------------------
var keyVaultName = toLower('${environmentName}-kv')

resource keyVault 'Microsoft.KeyVault/vaults@2021-11-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    accessPolicies: []
    enableRbacAuthorization: true
    enableSoftDelete: false
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    tenantId: subscription().tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource appServiceKeyVaultAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid('Key Vault Secret User', appService.name, subscription().subscriptionId)
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // this is the role "Key Vault Secrets User"
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  name: 'sqlServerConnectionString'
  parent: keyVault
  properties: {
    contentType:'ConnectionString'
    attributes: {
      enabled: true
    }
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${sqlDb.name};User Id=${sqlAdminUser};Password=${sqlAdminPwd};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}
