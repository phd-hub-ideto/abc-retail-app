using ABCRetailApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetailApp.Controllers;

public class FileStorageController : Controller
{
    private readonly IConfiguration _configuration;

    public FileStorageController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/file/list";

        using var client = new HttpClient();

        var response = await client.GetAsync(functionUrl);

        if (!response.IsSuccessStatusCode)
        {
            return View(new List<FileShareItem>());
        }

        var json = await response.Content.ReadAsStringAsync();

        var files = JsonSerializer.Deserialize<List<FileShareItem>>(json, Constants.SharedJsonSerializerOptions);

        return View(files);
    }

    public IActionResult Upload() => View();

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file != null)
        {
            var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/file/upload";

            using var content = new MultipartFormDataContent
            {
                { new StreamContent(file.OpenReadStream()), "file", file.FileName }
            };

            using var client = new HttpClient();

            await client.PostAsync(functionUrl, content);

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public async Task<IActionResult> Download(string fileName)
    {
        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/file/download/{fileName}";

        using var client = new HttpClient();

        var response = await client.GetAsync(functionUrl);

        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction(nameof(Index));
        }

        var stream = await response.Content.ReadAsStreamAsync();

        return File(stream, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> Delete(string fileName)
    {
        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/file/delete/{fileName}";

        using var client = new HttpClient();

        await client.DeleteAsync(functionUrl);

        return RedirectToAction(nameof(Index));
    }
}