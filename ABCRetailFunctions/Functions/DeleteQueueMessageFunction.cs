using ABCRetailApp;
using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Text.Json;

namespace ABCRetailFunctions.Functions;

public class DeleteQueueMessageFunction(
    AzureQueueStorageService queueService)
{
    [Function("DeleteQueueMessage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "queue/delete")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();

        var data = JsonSerializer.Deserialize<DeleteMessageModel>(body, Constants.SharedJsonSerializerOptions);

        if (data == null || string.IsNullOrEmpty(data.MessageId) || string.IsNullOrEmpty(data.PopReceipt))
        {
            return new BadRequestObjectResult("Invalid request.");
        }

        await queueService.DeleteMessageAsync(data.MessageId, data.PopReceipt);

        return new OkResult();
    }
}