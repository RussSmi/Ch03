using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Serverless.Openhack
{
    public class GetRating
    {
        [FunctionName("GetRating")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rating/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName: "ch03db",
                collectionName: "ch03collection",
                ConnectionStringSetting = "CosmosDBConnectionString",
                SqlQuery = "select * from c where c.id = {id}"
            )] IEnumerable<Rating> ratingItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (ratingItem == null)
            {
                log.LogInformation($"rating not found");
            }
           
            return new OkObjectResult(ratingItem);
        }
    }
}
