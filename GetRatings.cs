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
using Newtonsoft.Json.Linq;
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
            )] IEnumerable<JObject> allRatings,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];

            if (string.IsNullOrEmpty(userId))
            {
                return new BadRequestObjectResult(@"Please supply userId query parameter.");
            }

            var userRatings = allRatings.Where(r => r.Value<string>(@"userId") == userId);

            if (userRatings.Any())
            {
                return new OkObjectResult(userRatings);
            }
            else
            {
                return new NotFoundObjectResult($@"No ratings found for user '{userId}'");
            }
        }
    }
}
