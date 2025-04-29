using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ABCRetailFunctions.Functions;

public class DownloadFileFunction(
    AzureFileStorageService fileStorageService)
{
    [Function("DownloadFile")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "file/download/{fileName}")] HttpRequest req,
        string fileName)
    {
        var stream = await fileStorageService.DownloadFileAsync(fileName);

        return new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }
}