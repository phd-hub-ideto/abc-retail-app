using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetailApp.Controllers;

public class CustomerProfileController : Controller
{
    private readonly AzureTableService _azureTableService;
    private readonly IConfiguration _configuration;

    public CustomerProfileController(
        AzureTableService azureTableService,
        IConfiguration configuration)
    {
        _azureTableService = azureTableService;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var customers = await _azureTableService.GetAllCustomerProfilesAsync();

        return View(customers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Add new customer profile
    [HttpPost]
    public async Task<IActionResult> CreateProfile(CustomerProfile profile)
    {
        using var client = new HttpClient();
        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/storeprofile";

        var json = JsonSerializer.Serialize(profile);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(functionUrl, content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, $"Error: {error}");
        return View("Create", profile);
    }

    // View customer profile
    [HttpGet]
    public async Task<IActionResult> ViewProfile(string partitionKey, string rowKey)
    {
        var profile = await _azureTableService.GetCustomerProfileAsync(partitionKey, rowKey);

        return View(profile);
    }
}