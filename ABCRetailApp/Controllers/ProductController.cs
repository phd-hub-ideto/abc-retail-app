using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetailApp.Controllers;

public class ProductController : Controller
{
    private readonly AzureTableService _azureTableService;
    private readonly AzureBlobService _azureBlobService;
    private readonly IConfiguration _configuration;

    public ProductController(
        AzureTableService azureTableService,
        AzureBlobService azureBlobService,
        IConfiguration configuration)
    {
        _azureTableService = azureTableService;
        _azureBlobService = azureBlobService;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _azureTableService.GetAllProductsAsync();

        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Add new product
    [HttpPost]
    public async Task<IActionResult> AddProduct(Product product)
    {
        if (ModelState.IsValid)
        {
            using var client = new HttpClient();
            HttpResponseMessage response;

            string functionUrl = string.Empty;

            // Retrieve the image from the request directly
            var image = Request.Form.Files.GetFile("Image");

            // Generate a unique file name for the image
            if (image != null)
            {
                using var content = new MultipartFormDataContent();
                using var stream = image.OpenReadStream();

                content.Add(new StreamContent(stream), "file", image.FileName);

                functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/uploadimage";

                response = await client.PostAsync(functionUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Image upload failed.");

                    return View("Create", product);
                }

                var imageUrl = await response.Content.ReadAsStringAsync();

                product.ImageUrl = imageUrl.Trim('"'); // Remove quotes from JSON string
            }

            functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/storeproduct";

            var productJson = JsonSerializer.Serialize(product);

            var productContent = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");

            response = await client.PostAsync(functionUrl, productContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(string.Empty, $"Error storing product: {error}");
        }

        return View("Create", product);
    }

    [HttpGet]
    public async Task<IActionResult> ViewProduct(string partitionKey, string rowKey)
    {
        var product = await _azureTableService.GetProductAsync(partitionKey, rowKey);

        return View(product);
    }

    // Upload image
    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        using (var stream = file.OpenReadStream())
        {
            await _azureBlobService.UploadImageAsync(stream, file.FileName);
        }
        return RedirectToAction("Index");
    }
}