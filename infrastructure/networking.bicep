param systemName string
param defaultResourceName string

var vnetAddressPrefixes = [
  '10.0.0.0/16'
]

var sqlDataSubnetName = 'SqlData'
var websiteSubnetName = 'Website'

var subnets = [
  //inline services has (10.0) /24 allows 256 addresses, and allows 256 subnets
  {
    name: sqlDataSubnetName
    prefix: '10.0.0.0/24'
    privateEndpointNetworkPolicies: 'Disabled'
    privateLinkServiceNetworkPolicies: 'Enabled'
    serviceEndpoints: []
    delegations: []
  }
  {
    name: websiteSubnetName
    prefix: '10.0.1.0/24'
    privateEndpointNetworkPolicies: 'Enabled'
    privateLinkServiceNetworkPolicies: 'Enabled'
    serviceEndpoints: []
    delegations: [
      {
        name: 'delegation'
        properties: {
          serviceName: 'Microsoft.Web/serverFarms'
        }
      }
    ]
  }
]

module vNetModule 'Network/virtualNetworks.bicep' = {
  name: 'vNetModule'
  params: {
    defaultResourceName: defaultResourceName
    addressPrefixes: vnetAddressPrefixes
    subnets: subnets
  }
}

module privateSqlDnsZoneModule 'Network/linkedPrivateDnsZones.bicep' = {
  name: 'privateSqlDnsZoneModule'
  params: {
    privateDnsZoneName: 'privatelink${environment().suffixes.sqlServerHostname}'
    virtualNetworkId: vNetModule.outputs.subscriptionResourceId
    linkName: '${systemName}-sql-vnet-link'
  }
}

output vnetSubscriptionResourceId string = vNetModule.outputs.subscriptionResourceId
output vNetName string = vNetModule.outputs.resourceName
output sqlDataSubnetName string = sqlDataSubnetName
output websiteSubnetName string = websiteSubnetName
