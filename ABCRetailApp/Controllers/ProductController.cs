using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers;

public class ProductController : Controller
{
    private readonly AzureTableService _azureTableService;
    private readonly AzureBlobService _azureBlobService;

    public ProductController(AzureTableService azureTableService, AzureBlobService azureBlobService)
    {
        _azureTableService = azureTableService;
        _azureBlobService = azureBlobService;
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
            // Retrieve the image from the request directly
            var image = Request.Form.Files.GetFile("Image");

            // Generate a unique file name for the image
            if (image != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                // Upload the image to Blob Storage
                using (var stream = image.OpenReadStream())
                {
                    await _azureBlobService.UploadImageAsync(stream, fileName);
                }

                // Save the image URL in the product
                product.ImageUrl = $"https://{_azureBlobService.GetBlobContainerClient().AccountName}.blob.core.windows.net/{_azureBlobService.GetContainerName()}/{fileName}";
            }

            // Set PartitionKey and RowKey for the product
            product.PartitionKey = "product";  // You can adjust based on your requirements
            product.RowKey = Guid.NewGuid().ToString(); // Unique identifier for the product

            // Add the product to Azure Table
            await _azureTableService.AddProductAsync(product);

            return RedirectToAction("Index");
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