using ABCRetailApp;
using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ABCRetailFunctions.Functions;

public class StoreCustomerProfileFunction(
    ILogger<StoreCustomerProfileFunction> logger,
    AzureTableService azureTableService)
{
    [Function("StoreCustomerProfile")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "storeprofile")] HttpRequest req)
    {
        logger.LogInformation("Received a request to store a customer profile.");

        // Read request body
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var profile = JsonSerializer.Deserialize<CustomerProfile>(requestBody, Constants.SharedJsonSerializerOptions);

        // Validate
        if (profile == null || string.IsNullOrEmpty(profile.Email))
        {
            return new BadRequestObjectResult("Invalid profile.");
        }

        profile.PartitionKey = "customer";
        profile.RowKey = Guid.NewGuid().ToString();

        await azureTableService.AddCustomerProfileAsync(profile);

        return new OkObjectResult($"Profile for {profile.FullName} stored.");
    }
}