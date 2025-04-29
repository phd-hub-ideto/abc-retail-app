using ABCRetailApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ABCRetailFunctions.Functions;

public class DeleteFileFunction(
    AzureFileStorageService fileStorageService)
{
    [Function("DeleteFile")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "file/delete/{fileName}")] HttpRequest req,
        string fileName)
    {
        await fileStorageService.DeleteFileAsync(fileName);

        return new OkResult();
    }
}