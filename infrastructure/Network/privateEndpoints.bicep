param defaultResourceName string
param sqlServerId string
param vnetResourceId string
param vnetResourceGroup string
param subnet string

var resourceName = '${defaultResourceName}-pe'

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' existing = {
  name: 'privatelink${environment().suffixes.sqlServerHostname}'
  scope: resourceGroup(vnetResourceGroup)
}

resource privateEndpoint 'Microsoft.Network/privateEndpoints@2021-05-01' = {
  name: resourceName
  location: resourceGroup().location
  properties: {
    privateLinkServiceConnections: [
      {
        name: resourceName
        properties: {
          privateLinkServiceId: sqlServerId
          groupIds: [
            'sqlServer'
          ]
          privateLinkServiceConnectionState: {
            status: 'Approved'
            actionsRequired: 'None'
            description: 'Auto-Approved'
          }
        }
      }
    ]
    subnet: {
      id: '${vnetResourceId}/subnets/${subnet}'
    }
  }
}

resource dns 'Microsoft.Network/privateEndpoints/privateDnsZoneGroups@2021-05-01' = {
  name: 'default'
  parent: privateEndpoint
  properties: {
    privateDnsZoneConfigs: [
      {
        name: 'privatednszonelink'
        properties: {
          privateDnsZoneId: privateDnsZone.id
        }
      }
    ]
  }
}
