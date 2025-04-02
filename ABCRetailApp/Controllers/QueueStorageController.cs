using ABCRetailApp.Models;
using ABCRetailApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailApp.Controllers;

public class QueueStorageController : Controller
{
    private readonly AzureQueueStorageService _queueStorageService;

    public QueueStorageController(AzureQueueStorageService queueStorageService)
    {
        _queueStorageService = queueStorageService;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _queueStorageService.ReceiveMessagesAsync();
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
            await _queueStorageService.SendMessageAsync(message);
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
        await _queueStorageService.DeleteMessageAsync(messageId, popReceipt);
        return RedirectToAction(nameof(Index));
    }
}