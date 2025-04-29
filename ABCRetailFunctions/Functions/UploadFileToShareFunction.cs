using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ABCRetailFunctions.Functions;

public class UploadFileToShareFunction(
    AzureFileStorageService fileStorageService)
{
    [Function("UploadFileToShare")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "file/upload")] HttpRequest req)
    {
        var form = await req.ReadFormAsync();
        var file = form.Files.FirstOrDefault();

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("No file uploaded.");
        }

        using var stream = file.OpenReadStream();
        await fileStorageService.UploadFileAsync(file.FileName, stream);

        return new OkObjectResult("File uploaded.");
    }
}