param location string
param sqlAdminUsername string
@secure()
param sqlAdminPassword string

var acrName = 'showcaseacr${uniqueString(resourceGroup().id)}'
var aksClusterName = 'showcase-aks-cluster'
var keyVaultName = 'showcase-kv-${uniqueString(resourceGroup().id)}'
// NOVO: Ime za naš namjenski identitet
var kvIdentityName = 'kv-identity-${uniqueString(resourceGroup().id)}'

// SQL Server
resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: 'showcase-sql-server-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
  }
}

// SQL Baza Podataka
resource sqlDatabase 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: 'AzureShowcaseDb'
  location: location
  sku: {
    name: 'Basic'
  }
}

// Azure Container Registry (ACR)
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

// Azure Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
  }
}

// Azure Kubernetes Service (AKS)
resource aksCluster 'Microsoft.ContainerService/managedClusters@2022-09-01' = {
  name: aksClusterName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
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
    addonProfiles: {
      azureKeyvaultSecretsProvider: {
        enabled: true
        config: {
          enableSecretRotation: 'true'
        }
      }
    }
  }
}

// --- NOVI DIO ZA ISPRAVNE OVLASTI ---

// 1. Kreiramo namjenski User-Assigned Managed Identity
resource kvIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: kvIdentityName
  location: location
}

// 2. Dajemo tom novom identitetu ovlasti da čita tajne iz Key Vaulta
resource keyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: keyVault
  name: guid(keyVault.id, kvIdentity.id, 'KeyVaultSecretsUser')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Uloga: Key Vault Secrets User
    principalId: kvIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

// 3. Dajemo AKS-ovom Kubelet identitetu ovlasti da koristi naš novi identitet
resource identityOperator 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: kvIdentity
  name: guid(kvIdentity.id, aksCluster.properties.identityProfile.kubeletidentity.objectId, 'ManagedIdentityOperator')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'f1a07417-d97a-45cb-824c-7a7467783830') // Uloga: Managed Identity Operator
    principalId: aksCluster.properties.identityProfile.kubeletidentity.objectId
    principalType: 'ServicePrincipal'
  }
}

// --- OUTPUT ---
// Ispisujemo Client ID našeg novog identiteta, trebat će nam
output identityClientId string = kvIdentity.properties.clientId