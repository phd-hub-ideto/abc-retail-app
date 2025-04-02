using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers;

public class FileStorageController : Controller
{
    private readonly AzureFileStorageService _fileStorageService;

    public FileStorageController(AzureFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<IActionResult> Index()
    {
        var files = await _fileStorageService.ListFilesAsync();
        return View(files);
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null)
        {
            using var stream = file.OpenReadStream();
            await _fileStorageService.UploadFileAsync(file.FileName, stream);
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    public async Task<IActionResult> Download(string fileName)
    {
        var stream = await _fileStorageService.DownloadFileAsync(fileName);
        return File(stream, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> Delete(string fileName)
    {
        await _fileStorageService.DeleteFileAsync(fileName);
        return RedirectToAction(nameof(Index));
    }
}
