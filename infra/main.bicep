targetScope = 'subscription'

@description('Naziv Resource Grupe.')
param resourceGroupName string = 'AzureShowcase-RG'

@description('Lokacija za resurse.')
param location string = 'northeurope'

@description('Korisničko ime za SQL admina.')
param sqlAdminUsername string = 'sqladmin'

@description('Lozinka za SQL admina.')
@secure()
param sqlAdminPassword string

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: resourceGroupName
  location: location
}

module appResources 'app.bicep' = {
  name: 'AppResourcesDeployment'
  scope: rg 
  params: {
    location: location
    sqlAdminUsername: sqlAdminUsername
    sqlAdminPassword: sqlAdminPassword
  }
}