@description('The location wher you want to create the resources.')
param location string = resourceGroup().location

@description('The name of the environment. It will be used to create the name of the resources in the resource group.')
@minLength(3)
param environmentName string = 'nsgDemo${uniqueString(resourceGroup().id)}'

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

//----------------------------------------------------------
// Application Security Groups
//----------------------------------------------------------
resource frontEndASG 'Microsoft.Network/applicationSecurityGroups@2022-01-01' = {
  name: 'FrontEnd-asg'
  location: location
}

resource backEndASG 'Microsoft.Network/applicationSecurityGroups@2022-01-01' = {
  name: 'BackEnd-asg'
  location: location
}

resource databaseASG 'Microsoft.Network/applicationSecurityGroups@2022-01-01' = {
  name: 'Database-asg'
  location: location
}

//----------------------------------------------------------
// Network Security Group
//----------------------------------------------------------
var nsgName = '${environmentName}-nsg'

resource securityGroup 'Microsoft.Network/networkSecurityGroups@2021-02-01' = {
  name: nsgName
  location: location
  properties: {
    securityRules: [
      {
        name: 'DenyVnetInBound'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
          access: 'Deny'
          priority: 4000
          direction: 'Inbound'
          sourcePortRanges: []
          destinationPortRanges: []
          sourceAddressPrefixes: []
          destinationAddressPrefixes: []
        }
      }
      {
        name: 'AllowInternetToFrontEnd'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          sourceAddressPrefix: 'Internet'
          access: 'Allow'
          priority: 2000
          direction: 'Inbound'
          sourcePortRanges: []
          destinationPortRanges: []
          sourceAddressPrefixes: []
          destinationAddressPrefixes: []
          destinationApplicationSecurityGroups: [
            {
              id: frontEndASG.id
            }
          ]
        }
      }
      {
        name: 'AllowFrontEndToBackEnd'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          access: 'Allow'
          priority: 2010
          direction: 'Inbound'
          sourcePortRanges: []
          destinationPortRanges: []
          sourceAddressPrefixes: []
          destinationAddressPrefixes: []
          sourceApplicationSecurityGroups: [
            {
              id: frontEndASG.id
            }
          ]
          destinationApplicationSecurityGroups: [
            {
              id: backEndASG.id
            }
          ]
        }
      }
      {
        name: 'AllowBackEndToDatabase'
        properties: {
          protocol: '*'
          sourcePortRange: '*'
          destinationPortRange: '*'
          access: 'Allow'
          priority: 2020
          direction: 'Inbound'
          sourcePortRanges: []
          destinationPortRanges: []
          sourceAddressPrefixes: []
          destinationAddressPrefixes: []
          sourceApplicationSecurityGroups: [
            {
              id: backEndASG.id
            }
          ]
          destinationApplicationSecurityGroups: [
            {
              id: databaseASG.id
            }
          ]
        }
      }
    ]
  }
}

resource jitVmFERule 'Microsoft.Network/networkSecurityGroups/securityRules@2022-01-01' = {
  name: 'SecurityCenter-JITRule-vmFE'
  parent: securityGroup
  properties: {
    description: 'ASC JIT Network Access rule for policy \'default\' of VM \'vmFE\'.'
    protocol: '*'
    sourcePortRange: '*'
    destinationPortRange: '3389'
    sourceAddressPrefix: '*'
    destinationAddressPrefix: vmFENic.properties.ipConfigurations[0].properties.privateIPAddress
    access: 'Deny'
    priority: 1000
    direction: 'Inbound'
    sourcePortRanges: []
    destinationPortRanges: []
    sourceAddressPrefixes: []
    destinationAddressPrefixes: []
  }
}

resource jitVmBERule 'Microsoft.Network/networkSecurityGroups/securityRules@2022-01-01' = {
  name: 'SecurityCenter-JITRule-vmBE'
  parent: securityGroup
  properties: {
    description: 'ASC JIT Network Access rule for policy \'default\' of VM \'vmBE\'.'
    protocol: '*'
    sourcePortRange: '*'
    destinationPortRange: '3389'
    sourceAddressPrefix: '*'
    destinationAddressPrefix: vmBENic.properties.ipConfigurations[0].properties.privateIPAddress
    access: 'Deny'
    priority: 1010
    direction: 'Inbound'
    sourcePortRanges: []
    destinationPortRanges: []
    sourceAddressPrefixes: []
    destinationAddressPrefixes: []
  }
}

resource jitVmDBRule 'Microsoft.Network/networkSecurityGroups/securityRules@2022-01-01' = {
  name: 'SecurityCenter-JITRule-vmDB'
  parent: securityGroup
  properties: {
    description: 'ASC JIT Network Access rule for policy \'default\' of VM \'vmDB\'.'
    protocol: '*'
    sourcePortRange: '*'
    destinationPortRange: '3389'
    sourceAddressPrefix: '*'
    destinationAddressPrefix: vmDBNic.properties.ipConfigurations[0].properties.privateIPAddress
    access: 'Deny'
    priority: 1020
    direction: 'Inbound'
    sourcePortRanges: []
    destinationPortRanges: []
    sourceAddressPrefixes: []
    destinationAddressPrefixes: []
  }
}

//----------------------------------------------------------
//  Network
//----------------------------------------------------------
var networkName = '${environmentName}-vnet'
var subnetName = 'subnet1'
var vnetAddressSpace = '${vnetIpAddress}/${cidrMask}'
var subnetAddressSpace = '${vnetIpAddress}/${cidrMask + 1}'

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
//  VM - FrontEnd
//----------------------------------------------------------
var vmFEName = 'FrontEnd-vm'

resource vmFEPip 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${vmFEName}-PIP'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: toLower('${vmFEName}-${uniqueString(resourceGroup().id)}')
    }
  }
}

resource vmFENic 'Microsoft.Network/networkInterfaces@2021-02-01' = {
  name: '${vmFEName}-Nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: vmFEPip.id
          }
          subnet: {
            id: virtualNetwork.properties.subnets[0].id
          }
          applicationSecurityGroups: [
            {
              id: frontEndASG.id
            }
          ]
        }
      }
    ]
  }
}

resource vmFE 'Microsoft.Compute/virtualMachines@2021-03-01' = {
  name: vmFEName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_B1s'
    }
    osProfile: {
      computerName: vmFEName
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
          storageAccountType: 'Standard_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmFENic.id
        }
      ]
    }
  }
}

resource vmFEShutdown 'Microsoft.DevTestLab/schedules@2018-09-15' = {
  name: 'shutdown-computevm-${vmFEName}'
  location: location
  properties: {
    status: 'Enabled'
    taskType: 'ComputeVmShutdownTask'
    dailyRecurrence: {
      time: '1800'
    }
    timeZoneId: 'W. Europe Standard Time'
    notificationSettings: {
      status: 'Disabled'
      timeInMinutes: 30
      notificationLocale: 'en'
    }
    targetResourceId: vmFE.id
  }
}

resource vmFEPingEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmFEName}-EnablePING-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Import-Module NetSecurity\nNew-NetFirewallRule -Name Allow_Ping -DisplayName "Allow Ping"  -Description "Packet Internet Groper ICMPv4" -Protocol ICMPv4 -IcmpType 8 -Enabled True -Profile Any -Action Allow'
    }
  }
}

resource vmFEIISEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmFEName}-EnableIIS-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Install-WindowsFeature -name Web-Server -IncludeManagementTools\nRemove-Item C:\\inetpub\\wwwroot\\iisstart.htm\nAdd-Content -Path "C:\\inetpub\\wwwroot\\iisstart.htm" -Value $("Hello from " + $env:computername)'
    }
  }
}
//----------------------------------------------------------
//  VM - BackEnd
//----------------------------------------------------------
var vmBEName = 'BackEnd-vm'

resource vmBEPip 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${vmBEName}-PIP'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: toLower('${vmBEName}-${uniqueString(resourceGroup().id)}')
    }
  }
}

resource vmBENic 'Microsoft.Network/networkInterfaces@2021-02-01' = {
  name: '${vmBEName}-Nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: vmBEPip.id
          }
          subnet: {
            id: virtualNetwork.properties.subnets[0].id
          }
          applicationSecurityGroups: [
            {
              id: backEndASG.id
            }
          ]
        }
      }
    ]
  }
}

resource vmBE 'Microsoft.Compute/virtualMachines@2021-03-01' = {
  name: vmBEName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_B1s'
    }
    osProfile: {
      computerName: vmBEName
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
          storageAccountType: 'Standard_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmBENic.id
        }
      ]
    }
  }
}

resource vmBEShutdown 'Microsoft.DevTestLab/schedules@2018-09-15' = {
  name: 'shutdown-computevm-${vmBEName}'
  location: location
  properties: {
    status: 'Enabled'
    taskType: 'ComputeVmShutdownTask'
    dailyRecurrence: {
      time: '1800'
    }
    timeZoneId: 'W. Europe Standard Time'
    notificationSettings: {
      status: 'Disabled'
      timeInMinutes: 30
      notificationLocale: 'en'
    }
    targetResourceId: vmBE.id
  }
}

resource vmBEPingEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmBEName}-EnablePING-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Import-Module NetSecurity\nNew-NetFirewallRule -Name Allow_Ping -DisplayName "Allow Ping"  -Description "Packet Internet Groper ICMPv4" -Protocol ICMPv4 -IcmpType 8 -Enabled True -Profile Any -Action Allow'
    }
  }
}

resource vmBEIISEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmBEName}-EnableIIS-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Install-WindowsFeature -name Web-Server -IncludeManagementTools\nRemove-Item C:\\inetpub\\wwwroot\\iisstart.htm\nAdd-Content -Path "C:\\inetpub\\wwwroot\\iisstart.htm" -Value $("Hello from " + $env:computername)'
    }
  }
}
//----------------------------------------------------------
//  VM - Database
//----------------------------------------------------------
var vmDBName = 'Database-vm'

resource vmDBPip 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${vmDBName}-PIP'
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    publicIPAllocationMethod: 'Static'
    dnsSettings: {
      domainNameLabel: toLower('${vmDBName}-${uniqueString(resourceGroup().id)}')
    }
  }
}

resource vmDBNic 'Microsoft.Network/networkInterfaces@2021-02-01' = {
  name: '${vmDBName}-Nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: vmDBPip.id
          }
          subnet: {
            id: virtualNetwork.properties.subnets[0].id
          }
          applicationSecurityGroups: [
            {
              id: databaseASG.id
            }
          ]
        }
      }
    ]
  }
}

resource vmDB 'Microsoft.Compute/virtualMachines@2021-03-01' = {
  name: vmDBName
  location: location
  properties: {
    hardwareProfile: {
      vmSize: 'Standard_B1s'
    }
    osProfile: {
      computerName: vmDBName
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
          storageAccountType: 'Standard_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmDBNic.id
        }
      ]
    }
  }
}

resource vmDBShutdown 'Microsoft.DevTestLab/schedules@2018-09-15' = {
  name: 'shutdown-computevm-${vmDBName}'
  location: location
  properties: {
    status: 'Enabled'
    taskType: 'ComputeVmShutdownTask'
    dailyRecurrence: {
      time: '1800'
    }
    timeZoneId: 'W. Europe Standard Time'
    notificationSettings: {
      status: 'Disabled'
      timeInMinutes: 30
      notificationLocale: 'en'
    }
    targetResourceId: vmDB.id
  }
}

resource vmDBPingEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmDBName}-EnablePING-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Import-Module NetSecurity\nNew-NetFirewallRule -Name Allow_Ping -DisplayName "Allow Ping"  -Description "Packet Internet Groper ICMPv4" -Protocol ICMPv4 -IcmpType 8 -Enabled True -Profile Any -Action Allow'
    }
  }
}

resource vmDBIISEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmDBName}-EnableIIS-Script'
  location:location
  parent:vmFE
  properties:{
    source:{
      script:'Install-WindowsFeature -name Web-Server -IncludeManagementTools\nRemove-Item C:\\inetpub\\wwwroot\\iisstart.htm\nAdd-Content -Path "C:\\inetpub\\wwwroot\\iisstart.htm" -Value $("Hello from " + $env:computername)'
    }
  }
}
