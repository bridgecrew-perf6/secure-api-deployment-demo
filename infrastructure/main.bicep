targetScope = 'subscription'

@maxLength(8)
param systemName string
@maxLength(3)
param locationAbbreviation string
@allowed([
  'Dev'
  'Test'
  'Acc'
  'Prod'
])
param environmentName string

param basicAppSettings array = [
  {
    name: 'WEBSITE_RUN_FROM_PACKAGE'
    value: '1'
  }
]

@secure()
param sqlServerPassword string

@allowed([
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GRS'
  'Standard_GZRS'
  'Standard_LRS'
  'Standard_RAGRS'
  'Standard_RAGZRS'
  'Standard_ZRS'
])
param storageAccountSku string = 'Standard_LRS'

param sqlServerDatabaseSku object = {
  name: 'Standard'
  tier: 'Standard'
  capacity: 10
}

param deployTimeKeyVaultName string = 'deploy-time-kv'
param deployTimeResourceGroup string = 'Deploy-Time'

var resourceGroupName = toLower('${systemName}-${environmentName}-${locationAbbreviation}')
var defaultResourceName = toLower('${systemName}-${environmentName}-${locationAbbreviation}')

resource targetResourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: deployment().location
}

resource deployTimeKeyVault 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: deployTimeKeyVaultName
  scope: resourceGroup(deployTimeResourceGroup)
}

module networkingModule 'networking.bicep' = {
  name: 'networkingModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
    systemName: systemName
  }
}

module applicationInsightModule 'Insights/components.bicep' = {
  name: 'applicationInsightModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
  }
}

module storageAccountModule 'Storage/storageAccounts.bicep' = {
  name: 'storageModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
    skuName: storageAccountSku
  }
}

module appServicePlan 'Web/serverFarms.bicep' = {
  name: 'appServicePlan'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
  }
}

module webAppModule 'Web/sites.bicep' = {
  name: 'webAppModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
    appServicePlanId: appServicePlan.outputs.id
    subnetName: networkingModule.outputs.websiteSubnetName
    vnetResourceId: networkingModule.outputs.vnetSubscriptionResourceId
    kind: 'app'
  }
}

module keyVaultModule 'KeyVault/vault.bicep' = {
  name: 'keyVaultModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
  }
}

module sqlServerModule 'Sql/servers.bicep' = {
  name: 'sqlServerModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
    administratorPassword: sqlServerPassword
  }
}

module sqlServerDatabaseModule 'Sql/servers/database.bicep' = {
  name: 'sqlServerDatabaseModule'
  scope: targetResourceGroup
  params: {
    sku: sqlServerDatabaseSku
    defaultResourceName: defaultResourceName
    sqlServerName: sqlServerModule.outputs.sqlServerName
  }
}

module sqlServerPrivateEndpointModule 'Network/privateEndpoints.bicep' = {
  name: 'sqlServerPrivateEndpointModule'
  scope: targetResourceGroup
  params: {
    defaultResourceName: defaultResourceName
    sqlServerId: sqlServerModule.outputs.resourceId
    vnetResourceId: networkingModule.outputs.vnetSubscriptionResourceId
    vnetResourceGroup: targetResourceGroup.name
    subnet: networkingModule.outputs.sqlDataSubnetName
  }
}

module storageAccountSecretModule 'KeyVault/vaults/secrets.bicep' = {
  dependsOn: [
    keyVaultModule
    storageAccountModule
  ]
  name: 'storageAccountSecretModule'
  scope: targetResourceGroup
  params: {
    keyVault: keyVaultModule.outputs.keyVaultName
    secret: storageAccountModule.outputs.secret
  }
}

module sqlServerSecretModule 'KeyVault/vaults/sqlServerSecret.bicep' = {
  dependsOn: [
    keyVaultModule
    sqlServerModule
  ]
  name: 'sqlServerSecretModule'
  scope: targetResourceGroup
  params: {
    keyVault: keyVaultModule.outputs.keyVaultName
    secretName: 'SqlConnectionString'
    serverName: sqlServerModule.outputs.sqlServerName
    userName: sqlServerModule.outputs.sqlServerLogin
    password: deployTimeKeyVault.getSecret('SqlServerPassword')
    databaseName: sqlServerDatabaseModule.outputs.databaseName
  }
}

module keyVaultAccessPolicyModule 'KeyVault/vaults/accessPolicies.bicep' = {
  name: 'keyVaultAccessPolicyDeploy'
  dependsOn: [
    keyVaultModule
    webAppModule
  ]
  scope: targetResourceGroup
  params: {
    keyVaultName: keyVaultModule.outputs.keyVaultName
    principalId: webAppModule.outputs.servicePrincipal
  }
}

module websiteConfiguration 'Web/sites/config.bicep' = {
  name: 'websiteConfiguration'
  dependsOn: [
    keyVaultModule
    keyVaultAccessPolicyModule
  ]
  scope: targetResourceGroup
  params: {
    webAppName: webAppModule.outputs.webAppName
    appSettings: union(basicAppSettings, applicationInsightModule.outputs.appConfiguration, storageAccountSecretModule.outputs.keyVaultReference, sqlServerSecretModule.outputs.keyVaultReference)
  }
}
