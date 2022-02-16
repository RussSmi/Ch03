using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace Serverless.Openhack
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "ch03db",
                collectionName: "ch03collection",
                ConnectionStringSetting = "CosmosDBConnectionString"
            )] IAsyncCollector<Rating> ratingsOut,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Rating rating = JsonConvert.DeserializeObject<Rating>(requestBody);    

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://serverlessohproduct.trafficmanager.net");
                    var result = await client.GetAsync($"/api/GetProduct?productId={rating.productId}");
                    string resultBody = await result.Content.ReadAsStringAsync();
                    Product product = JsonConvert.DeserializeObject<Product>(resultBody);
                }
            }
            catch (System.Exception)
            {
                return new NotFoundObjectResult($"Could not find product with id: {rating.productId}");
            }       

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://serverlessohuser.trafficmanager.net");
                    var result = await client.GetAsync($"/api/GetUser?userId={rating.userId}");
                    string resultBody = await result.Content.ReadAsStringAsync();
                    User user = JsonConvert.DeserializeObject<User>(resultBody);
                }
            }
            catch (System.Exception ex)
            {
                return new NotFoundObjectResult($"Could not find user with id: {rating.userId}");
            }

             // Add a property called id with a GUID value
            rating.id = Guid.NewGuid().ToString();

            // Add a property called timestamp with the current UTC date time
            rating.timeStamp = DateTime.UtcNow;

            // Validate that the rating field is an integer from 0 to 5
            if (rating.rating < 0 || rating.rating > 5)
            {
                return new BadRequestObjectResult("Bad request, Rating must be between 0 and 5 ");
            }

            // Use a data service to store the ratings information to the backend
            await ratingsOut.AddAsync(rating);

            return new OkObjectResult(rating);
        }
    }
}
