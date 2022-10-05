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

resource vmShutdown 'Microsoft.DevTestLab/schedules@2018-09-15' = {
  name: 'shutdown-computevm-${vmName}'
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
    targetResourceId: vm.id
  }
}


resource vmPingEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmName}-EnablePING-Script'
  location:location
  parent:vm
  properties:{
    source:{
      script:'Import-Module NetSecurity\nNew-NetFirewallRule -Name Allow_Ping -DisplayName "Allow Ping"  -Description "Packet Internet Groper ICMPv4" -Protocol ICMPv4 -IcmpType 8 -Enabled True -Profile Any -Action Allow'
    }
  }
}

resource vmFEIISEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01'={
  name: '${vmName}-EnableIIS-Script'
  location:location
  parent:vm
  properties:{
    source:{
      script:'Install-WindowsFeature -name Web-Server -IncludeManagementTools\nRemove-Item C:\\inetpub\\wwwroot\\iisstart.htm\nAdd-Content -Path "C:\\inetpub\\wwwroot\\iisstart.htm" -Value $("Hello from " + $env:computername)'
    }
  }
}
