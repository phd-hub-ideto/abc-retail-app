using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers;

public class CustomerProfileController : Controller
{
    private readonly AzureTableService _azureTableService;

    public CustomerProfileController(AzureTableService azureTableService)
    {
        _azureTableService = azureTableService;
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
        profile.PartitionKey = "customer";  // Group all customer data under one partition
        profile.RowKey = Guid.NewGuid().ToString();
        await _azureTableService.AddCustomerProfileAsync(profile);
        return RedirectToAction("Index");
    }

    // View customer profile
    [HttpGet]
    public async Task<IActionResult> ViewProfile(string partitionKey, string rowKey)
    {
        var profile = await _azureTableService.GetCustomerProfileAsync(partitionKey, rowKey);
       
        return View(profile);
    }
}