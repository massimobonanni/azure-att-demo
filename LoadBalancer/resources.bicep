param numberOfVMs int = 2

@description('Admin username for the Virtual Machines.')
param adminUsername string


@description('Demo name is used to create the resource group name and resources names')
param demoName string = 'LB'

@description('Azure location in which you create the resources')
param location string= resourceGroup().location

@description('Password for the Virtual Machine.')
@minLength(12)
@secure()
param adminPassword string

var publicIPAllocationMethod = 'Static'
var publicIpSku = 'Standard'
var OSVersion = '2022-datacenter'
var vmSize = 'Standard_B1s'

var virtualNetworkName = '${demoName}-VNET'
var networkSecurityGroupName = '${demoName}-nsg'
var loadBalancerName = '${demoName}-LB'

resource virtualNetwork 'Microsoft.Network/virtualNetworks@2022-01-01' = {
  name: virtualNetworkName
  location: location
  properties: {
    addressSpace: {
      addressPrefixes: [
        '172.17.0.0/16'
      ]
    }
    subnets: [
      {
        name: 'default'
        properties: {
          addressPrefix: '172.17.0.0/24'
          networkSecurityGroup: {
            id: securityGroup.id
          }
        }
      }
    ]
  }
  dependsOn: []
}

resource vmPips 'Microsoft.Network/publicIPAddresses@2021-02-01' = [for i in range(1, numberOfVMs): {
  name: '${demoName}-vm${i}-PIP'
  location: location
  sku: {
    name: publicIpSku
  }
  properties: {
    publicIPAllocationMethod: publicIPAllocationMethod
    dnsSettings: {
      domainNameLabel: toLower('${demoName}-${uniqueString(resourceGroup().id, '${demoName}-vm${i}')}')
    }
  }
  dependsOn: [
    loadBalancer
  ]
}]

resource securityGroup 'Microsoft.Network/networkSecurityGroups@2021-02-01' = {
  name: networkSecurityGroupName
  location: location
  properties: {
    securityRules: [
      {
        name: 'default-allow-3389'
        properties: {
          priority: 1000
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '3389'
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'httpAllowed'
        properties: {
          priority: 1100
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '80'
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'httpsAllowed'
        properties: {
          priority: 1200
          access: 'Allow'
          direction: 'Inbound'
          destinationPortRange: '443'
          protocol: 'Tcp'
          sourcePortRange: '*'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

resource vmNICs 'Microsoft.Network/networkInterfaces@2021-02-01' = [for i in range(1, numberOfVMs): {
  name: '${demoName}-vm${i}-Nic'
  location: location
  properties: {
    ipConfigurations: [
      {
        name: 'ipconfig${i}'
        properties: {
          privateIPAllocationMethod: 'Dynamic'
          publicIPAddress: {
            id: vmPips[i - 1].id
          }
          subnet: {
            id: resourceId('Microsoft.Network/virtualNetworks/subnets', virtualNetwork.name, 'default')
          }
          loadBalancerBackendAddressPools: [
            {
              id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', loadBalancerName, 'BackendPool1')
            }
          ]
        }
      }
    ]
  }
  dependsOn: [
    vmPips[i - 1]
    loadBalancer
  ]
}]

resource vms 'Microsoft.Compute/virtualMachines@2021-03-01' = [for i in range(1, numberOfVMs): {
  name: '${demoName}-vm${i}'
  location: location
  properties: {
    hardwareProfile: {
      vmSize: vmSize
    }
    osProfile: {
      computerName: '${demoName}-vm${i}'
      adminUsername: adminUsername
      adminPassword: adminPassword
    }
    storageProfile: {
      imageReference: {
        publisher: 'MicrosoftWindowsServer'
        offer: 'WindowsServer'
        sku: OSVersion
        version: 'latest'
      }
      osDisk: {
        createOption: 'FromImage'
        managedDisk: {
          storageAccountType: 'StandardSSD_LRS'
        }
      }
    }
    networkProfile: {
      networkInterfaces: [
        {
          id: vmNICs[i - 1].id
        }
      ]
    }
  }
}]

resource loadBalancerPIP 'Microsoft.Network/publicIPAddresses@2021-02-01' = {
  name: '${demoName}-LB-PIP'
  location: location
  sku: {
    name: publicIpSku
  }
  properties: {
    publicIPAllocationMethod: publicIPAllocationMethod
    dnsSettings: {
      domainNameLabel: toLower('${demoName}-${uniqueString(resourceGroup().id, '${demoName}-LB')}')
    }
  }
}

resource loadBalancer 'Microsoft.Network/loadBalancers@2021-05-01' = {
  name: loadBalancerName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    frontendIPConfigurations: [
      {
        properties: {
          publicIPAddress: {
            id: loadBalancerPIP.id
          }
        }
        name: 'LoadBalancerFrontend'
      }
    ]
    backendAddressPools: [
      {
        name: 'BackendPool1'
      }
    ]
    loadBalancingRules: [
      {
        properties: {
          frontendIPConfiguration: {
            id: resourceId('Microsoft.Network/loadBalancers/frontendIpConfigurations', loadBalancerName, 'LoadBalancerFrontend')
          }
          backendAddressPool: {
            id: resourceId('Microsoft.Network/loadBalancers/backendAddressPools', loadBalancerName, 'BackendPool1')
          }
          probe: {
            id: resourceId('Microsoft.Network/loadBalancers/probes', loadBalancerName, 'lbprobe')
          }
          protocol: 'Tcp'
          frontendPort: 80
          backendPort: 80
          idleTimeoutInMinutes: 15
        }
        name: 'lbrule'
      }
    ]
    probes: [
      {
        properties: {
          protocol: 'Tcp'
          port: 80
          intervalInSeconds: 15
          numberOfProbes: 2
        }
        name: 'lbprobe'
      }
    ]
  }
  dependsOn: [
    virtualNetwork
  ]
}

output vmInfo array = [for i in range(1, numberOfVMs): {
  fqdn: vmPips[i - 1].properties.dnsSettings.fqdn
}]

