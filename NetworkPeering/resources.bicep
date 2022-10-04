@description('The location where you want to create the resources.')
param location string = resourceGroup().location

@description('The base ip address for the network address space.')
param vnetIpAddress string 

@description('The CIDR mask number for the VNet.')
param cidrMask int

@description('Admin username for vms.')
param adminUsername string

@description('Admin password for vms.')
@minLength(12)
@secure()
param adminPassword string

var networkName = 'VNet-${location}'
var subnetName='subnet1'
var vnetAddressSpace='${vnetIpAddress}/${cidrMask}'
var subnetAddressSpace='${vnetIpAddress}/${cidrMask+1}'
var vmName = 'vm-${location}'

var nsgName='${location}-nsg'

//----------------------------------------------------------
// Network Security Group
//----------------------------------------------------------
resource securityGroup 'Microsoft.Network/networkSecurityGroups@2021-02-01' = {
  name: nsgName
  location: location
  properties: {
    securityRules: [
    ]
  }
}

//----------------------------------------------------------
//  Network
//----------------------------------------------------------
resource virtualNetwork 'Microsoft.Network/virtualNetworks@2022-01-01' = {
  name: networkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        vnetAddressSpace
      ]
    }
    subnets: [
      {
        name: subnetName
        properties: {
          addressPrefix: subnetAddressSpace
          networkSecurityGroup: {
            id: securityGroup.id
          }
        }
      }
    ]
  }
  dependsOn: []
}

//----------------------------------------------------------
//  VM
//----------------------------------------------------------
var diagStoragename='diag${uniqueString(resourceGroup().id)}${location}'

resource diagStorage 'Microsoft.Storage/storageAccounts@2021-06-01' =  {
  name: substring(diagStoragename,0,min(24,length(diagStoragename)))
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

resource vmPip 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${vmName}-PIP'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: toLower('${vmName}-${uniqueString(resourceGroup().id)}')
    }
  }
}

resource vmNIC 'Microsoft.Network/networkInterfaces@2021-02-01' =  {
  name: '${vmName}-Nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: vmPip.id
          }
          subnet: {
            id: virtualNetwork.properties.subnets[0].id 
          }
        }
      }
    ]
  }
}

resource vm 'Microsoft.Compute/virtualMachines@2021-03-01' =  {
  name: vmName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_B1s'
    }
    osProfile: {
      computerName: vmName
      adminUsername: adminUsername
      adminPassword: adminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: '2022-datacenter'
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType:  'Standard_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmNIC.id
        }
      ]
    }
    diagnosticsProfile: {
      bootDiagnostics: {
        enabled: true
        storageUri: diagStorage.properties.primaryEndpoints.blob
      }
    }
  }
}

// Shutdown schedule per vm
//{
//  "type": "microsoft.devtestlab/schedules",
//  "apiVersion": "2018-09-15",
//  "name": "[parameters('schedules_shutdown_computevm_westeuropevm_name')]",
//  "location": "westeurope",
//  "properties": {
//      "status": "Enabled",
//      "taskType": "ComputeVmShutdownTask",
//      "dailyRecurrence": {
//          "time": "1800"
//      },
//      "timeZoneId": "W. Europe Standard Time",
//      "notificationSettings": {
//          "status": "Disabled",
//          "timeInMinutes": 30,
//          "notificationLocale": "en"
//      },
//      "targetResourceId": "[parameters('virtualMachines_WestEuropeVM_externalid')]"
//  }
//}
