using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ABCRetailFunctions.Functions;

public class UploadProductImageFunction(
    ILogger<UploadProductImageFunction> logger,
    AzureBlobService blobService)
{
    [Function("UploadProductImage")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "uploadimage")] HttpRequest req)
    {
        logger.LogInformation("UploadProductImageFunction triggered.");

        var file = req.Form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file uploaded.");
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        using var stream = file.OpenReadStream();

        await blobService.UploadImageAsync(stream, fileName);

        var imageUrl = $"https://{blobService.GetBlobContainerClient().AccountName}.blob.core.windows.net/{blobService.GetContainerName()}/{fileName}";

        return new OkObjectResult(imageUrl);
    }
}