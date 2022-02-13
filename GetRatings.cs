using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Linq;
using System.Collections.Generic;

namespace Serverless.Openhack
{
    public class GetRatings
    {
        [FunctionName("GetRatings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ch03db",
                collectionName: "ch03collection",
                ConnectionStringSetting = "CosmosDBConnectionString"
            )] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ch03db", "ch03collection");

            IDocumentQuery<Rating> query = client.CreateDocumentQuery<Rating>(collectionUri)
                .Where(p => p.userId == userId)
                .AsDocumentQuery();

            var listOfRatings = new List<Rating>();

            while(query.HasMoreResults)
            {
                foreach(Rating result in await query.ExecuteNextAsync())
                {
                    listOfRatings.Add(result);
                }
            }
            return new OkObjectResult(listOfRatings);
        }
    }
}
