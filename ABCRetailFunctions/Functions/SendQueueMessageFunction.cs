using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class SendQueueMessageFunction(
    ILogger<SendQueueMessageFunction> logger,
    AzureQueueStorageService queueService)
{
    [Function("SendQueueMessage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "queue/send")] HttpRequest req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return new BadRequestObjectResult("Message body cannot be empty.");
        }

        await queueService.SendMessageAsync(body);

        logger.LogInformation("Message sent to queue.");

        return new OkResult();
    }
}