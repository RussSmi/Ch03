param loc string = resourceGroup().location

@minLength(3)
@maxLength(11)
param prefix string

@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
  'Standard_ZRS'
  'Premium_LRS'
  'Premium_ZRS'
  'Standard_GZRS'
  'Standard_RAGZRS'
])
param storageSKU string = 'Standard_LRS'

var uniqueStorageName = '${prefix}strg${uniqueString(resourceGroup().id)}'

resource stg 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: uniqueStorageName
  location: loc
  sku: {
    name: storageSKU
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
  }
}

resource appInsightsComponents 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: '${prefix}appinsights'
  location: loc
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

 
resource AppService 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: '${prefix}-app-service'
  location: loc
  sku: {
    tier: 'Dynamic'
    name: 'Y1'
  }
}

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-03-15' existing = {
 name: 'ch03cosmosacc'
 scope: resourceGroup('rg-open-hack-serverless')
}

var FunctionName = '${prefix}-function-app'
 
resource Function 'Microsoft.Web/sites@2021-01-15' = {
  name: FunctionName
  kind: 'Functionapp'
  location: loc
  properties: {
    serverFarmId: AppService.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
            name: 'FUNCTIONS_WORKER_RUNTIME'
            value: 'dotnet'
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${stg.name};AccountKey=${listKeys(stg.id, stg.apiVersion).keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${stg.name};AccountKey=${listKeys(stg.id, stg.apiVersion).keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: '${toLower(FunctionName)}files'
        }
        {
          name: 'CosmosDBConnectionString'
          value: 'AccountEndpoint=${cosmosAccount.properties.documentEndpoint};AccountKey=${cosmosAccount.listKeys().primaryMasterKey}'
          //cosmosAccount.listKeys().primaryMasterKey
          //listConnectionStrings(resourceId('Microsoft.DocumentDB/databaseAccounts', 'ch03cosmosacc'), '2021-04-15').connectionStrings[0].connectionString
          
        }
      ]
      use32BitWorkerProcess: false
      ftpsState: 'Disabled'
    }
  }
}

output storageEndpoint object = stg.properties.primaryEndpoints
