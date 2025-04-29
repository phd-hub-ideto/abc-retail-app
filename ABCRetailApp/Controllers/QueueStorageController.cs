using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetailApp.Controllers;

public class QueueStorageController : Controller
{
    private readonly IConfiguration _configuration;

    public QueueStorageController(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        using var client = new HttpClient();

        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/queue/receive";

        var response = await client.GetAsync(functionUrl);

        if (!response.IsSuccessStatusCode)
        {
            return View(new List<QueueMessageViewModel>());
        }

        var content = await response.Content.ReadAsStringAsync();

        var messages = JsonSerializer.Deserialize<List<QueueMessage>>(content, Constants.SharedJsonSerializerOptions);

        var viewModel = messages.Select(m => new QueueMessageViewModel
        {
            MessageId = m.MessageId,
            PopReceipt = m.PopReceipt,
            Content = m.Content // Use Content directly instead of Body.ToString()
        }).ToList();

        return View(viewModel);
    }

    public IActionResult SendMessage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            using var client = new HttpClient();

            var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/queue/send";

            var content = new StringContent(message, System.Text.Encoding.UTF8, "text/plain");

            await client.PostAsync(functionUrl, content);

            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    public IActionResult DeleteMessage(string messageId, string popReceipt)
    {
        var model = new Tuple<string, string>(messageId, popReceipt);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMessageConfirmed(string messageId, string popReceipt)
    {
        using var client = new HttpClient();

        var functionUrl = $"{_configuration.GetValue<string>(Constants.FunctionsBaseUrl)}/api/queue/delete";

        var json = new DeleteMessageModel
        {
            MessageId = messageId,
            PopReceipt = popReceipt
        };

        var payload = JsonSerializer.Serialize(json);

        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

        await client.PostAsync(functionUrl, content);

        return RedirectToAction(nameof(Index));
    }
}