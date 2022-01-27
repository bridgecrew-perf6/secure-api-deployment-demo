param keyVault string
param secretName string
param serverName string
param databaseName string
param userName string
@secure()
param password string

resource secretsLoop 'Microsoft.KeyVault/vaults/secrets@2021-04-01-preview' = {
  name: '${keyVault}/${secretName}'
  properties: {
    value: 'DATA SOURCE=tcp:${serverName}${environment().suffixes.sqlServerHostname},1433;USER ID=${userName};PASSWORD=${password};INITIAL CATALOG=${databaseName};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

output keyVaultReference array = [
  {
    name: secretName
    value: '@Microsoft.KeyVault(SecretUri=https://${keyVault}${environment().suffixes.keyvaultDns}/secrets/${secretName})'
  }
]
