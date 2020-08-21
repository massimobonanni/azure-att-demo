param
(
    [string]  [Parameter(Mandatory=$true)] $resourceGroup,
    [string]  [Parameter(Mandatory=$true)] [ValidateSet('Basic','Free','Isolated','Premium','PremiumV2','Shared','Standard')] $tier
)

$appServicePlans = Get-AzAppServicePlan -ResourceGroupName $resourceGroup

ForEach ($plan in $appServicePlans) {
    $planName=$plan.Name

    Set-AzAppServicePlan -Name $planName -Tier $tier -WorkerSize Small -ResourceGroupName $resourceGroup
}