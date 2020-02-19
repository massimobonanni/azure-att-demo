Login-AzAccount

Set-AzContext -Subscription "ATT Internal Subscription"

Write-Host "Retrieving SQL Firewall Rules"

$sqlFirewallRules=Get-AzSqlServerFirewallRule -ServerName sqlserverdemoatt -ResourceGroupName SQLServerDemo-rg

ForEach ($i in $sqlFirewallRules) {
    if ($i.FirewallRuleName.ToLower().StartsWith("webapp_")){
        Write-Host "Removing rule " $i.FirewallRuleName
        Remove-AzSqlServerFirewallRule -FirewallRuleName $i.FirewallRuleName -ServerName sqlserverdemoatt -Force -ResourceGroupName SQLServerDemo-rg
    }
}

$webapp=Get-AzWebApp -Name sqldemowebapp

ForEach ($i in $webapp.PossibleOutboundIpAddresses.Split(",")) {
    Write-Host "Adding rule to firewall for ip " $i
    $ruleName="WebApp_" + $i
    New-AzSqlServerFirewallRule -FirewallRuleName $ruleName -StartIpAddress $i -EndIpAddress $i -ServerName sqlserverdemoatt -ResourceGroupName SQLServerDemo-rg
}