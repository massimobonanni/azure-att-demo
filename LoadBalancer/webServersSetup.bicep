param numberOfVMs int = 2

@description('Azure location in which you create the resources')
param location string= resourceGroup().location


resource vms 'Microsoft.Compute/virtualMachines@2021-03-01' existing = [for i in range(1, numberOfVMs): {
  name: 'LB-vm${i}'
}]

resource vmFEIISEnabled 'Microsoft.Compute/virtualMachines/runCommands@2022-03-01' = [for i in range(1, numberOfVMs): {
  name: 'vm${i}-EnableIIS-Script'
  location: location
  parent: vms[i - 1]
  properties: {
    asyncExecution: false
    source: {
      script: '''
        Install-WindowsFeature -name Web-Server -IncludeManagementTools
        Remove-Item C:\\inetpub\\wwwroot\\iisstart.htm
        Add-Content -Path "C:\\inetpub\\wwwroot\\iisstart.htm" -Value $("Hello from " + $env:computername)  
      '''
    }
  }
  dependsOn: [
    vms[i - 1]
  ]
}]
