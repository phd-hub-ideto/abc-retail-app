using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ABCRetailFunctions.Functions;

public class ListFilesFunction(
    AzureFileStorageService fileStorageService)
{
    [Function("ListFiles")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "file/list")] HttpRequest req)
    {
        var files = await fileStorageService.ListFilesAsync();

        return new OkObjectResult(files);
    }
}