using ABCRetailApp;
using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ABCRetailFunctions.Functions;

public class StoreProductFunction(
    ILogger<StoreProductFunction> logger,
    AzureTableService azureTableService)
{
    [Function("StoreProduct")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "storeproduct")] HttpRequest req)
    {
        logger.LogInformation("Received a request to store a product.");

        var body = await new StreamReader(req.Body).ReadToEndAsync();

        var product = JsonSerializer.Deserialize<Product>(body, Constants.SharedJsonSerializerOptions);

        if (product == null || string.IsNullOrEmpty(product.Name))
        {
            return new BadRequestObjectResult("Invalid product data.");
        }

        // Assign keys if not already present (or let frontend do it)
        product.PartitionKey ??= "product";
        product.RowKey ??= Guid.NewGuid().ToString();

        await azureTableService.AddProductAsync(product);

        return new OkObjectResult($"Product '{product.Name}' stored.");
    }
}