param defaultResourceName string
param appServicePlanId string
param vnetResourceId string
param subnetName string
@allowed([
  'app'
  'functionapp'
])
param kind string = 'app'
var postfix = kind == 'app' ? 'app' : 'func'

var resourceName = '${defaultResourceName}-${postfix}'

resource webApp 'Microsoft.Web/sites@2020-12-01' = {
  name: resourceName
  location: resourceGroup().location
  kind: kind
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: true
      ftpsState: 'Disabled'
      http20Enabled: true
    }
    virtualNetworkSubnetId: '${vnetResourceId}/subnets/${subnetName}'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource vnet 'Microsoft.Web/sites/virtualNetworkConnections@2021-02-01' = {
  name: 'vnetIntegration'
  parent: webApp
  properties: {
    vnetResourceId: '${vnetResourceId}/subnets/${subnetName}'
    isSwift: true
  }
}

output servicePrincipal string = webApp.identity.principalId
output webAppName string = webApp.name
output resourceId string = webApp.id
output targetUrl string = webApp.properties.hostNames[0]
