param location string
param sqlAdminUsername string
@secure()
param sqlAdminPassword string

var acrName = 'showcaseacr${uniqueString(resourceGroup().id)}'
var aksClusterName = 'showcase-aks-cluster'

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: 'showcase-sql-server-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AzureShowcaseDb'
  location: location
  sku: {
    name: 'Basic'
  }
}

resource acr 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' = {
  name: acrName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource aksCluster 'Microsoft.ContainerService/managedClusters@2022-09-01' = {
  name: aksClusterName
  location: location
  properties: {
    dnsPrefix: aksClusterName
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: 1
        vmSize: 'Standard_B2s'
        osType: 'Linux'
        mode: 'System'
      }
    ]
    // **ISPRAVAK: Uklonili smo 'identity' blok kako bismo izbjegli BCP037 warning.**
    // AKS će automatski kreirati sistemski dodijeljen identitet.
  }
}