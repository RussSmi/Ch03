using System.Threading.Tasks;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ch03db",
                collectionName: "ch03collection",
                ConnectionStringSetting = "CosmosDBConnectionString",
                Id = "{Query.ratingId}",
                PartitionKey = "{Query.userId}"
            )] Rating ratingItem,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (ratingItem == null)
            {
                log.LogInformation($"rating not found");
            }
            else
            {
                log.LogInformation($"Found rating, id={ratingItem.id}");
            }

            return new OkObjectResult(ratingItem);
        }
    }
}
