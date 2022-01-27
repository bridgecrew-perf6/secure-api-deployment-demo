param privateDnsZoneName string
param linkName string
param virtualNetworkId string

resource privateDnsZone 'Microsoft.Network/privateDnsZones@2020-06-01' = {
  name: privateDnsZoneName
  location: 'global'
}

resource link 'Microsoft.Network/privateDnsZones/virtualNetworkLinks@2020-06-01' = {
  name: linkName
  location: 'global'
  parent: privateDnsZone
  properties: {
    virtualNetwork: {
      id: virtualNetworkId
    }
    registrationEnabled: false
  }
}

output name string = privateDnsZone.name
output privateDnsZoneId string = privateDnsZone.id
