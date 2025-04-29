using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ABCRetailFunctions.Functions;

public class ReceiveQueueMessagesFunction(
    AzureQueueStorageService queueService)
{
    [Function("ReceiveQueueMessages")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "queue/receive")] HttpRequest req)
    {
        var messages = await queueService.ReceiveMessagesAsync();

        return new OkObjectResult(messages);
    }
}