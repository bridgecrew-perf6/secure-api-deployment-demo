param defaultResourceName string

@secure()
param administratorPassword string

param location string = resourceGroup().location

var resourceName = '${defaultResourceName}-sql'
var administratorLogin = '${defaultResourceName}-admin'

resource sqlServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: resourceName
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    publicNetworkAccess: 'Disabled'
    restrictOutboundNetworkAccess: 'Disabled'
  }
}

output resourceId string = sqlServer.id
output sqlServerName string = sqlServer.name
output sqlServerLogin string = administratorLogin
