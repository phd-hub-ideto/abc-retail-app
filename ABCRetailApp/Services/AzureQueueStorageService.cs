using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetailApp.Services;

public class AzureQueueStorageService
{
    private readonly QueueClient _queueClient;

    public AzureQueueStorageService(IConfiguration configuration)
    {
        var queueServiceClient = new QueueServiceClient(configuration.GetConnectionString(Constants.AzureBlobStorage));
        _queueClient = queueServiceClient.GetQueueClient("orderprocessing");
        _queueClient.CreateIfNotExists();
    }

    public async Task SendMessageAsync(string message)
    {
        await _queueClient.SendMessageAsync(message);
    }

    // Adjusted to return a list of your custom QueueMessage objects
    public async Task<List<QueueMessage>> ReceiveMessagesAsync(int maxMessages = 10)
    {
        var receivedMessages = await _queueClient.ReceiveMessagesAsync(maxMessages, visibilityTimeout: TimeSpan.FromSeconds(1));

        // Map each Azure QueueMessage to your custom QueueMessage class
        return receivedMessages.Value.Select(m => new QueueMessage
        {
            MessageId = m.MessageId,
            PopReceipt = m.PopReceipt,
            Content = m.Body.ToString() // Convert the message body to string
        }).ToList();
    }

    public async Task DeleteMessageAsync(string messageId, string popReceipt)
    {
        await _queueClient.DeleteMessageAsync(messageId, popReceipt);
    }

    public async Task ClearMessagesAsync()
    {
        await _queueClient.ClearMessagesAsync();
    }

    public async Task<int> GetApproximateMessageCountAsync()
    {
        var properties = await _queueClient.GetPropertiesAsync();
        return properties.Value.ApproximateMessagesCount;
    }
}

public class QueueMessage
{
    public string MessageId { get; set; }
    public string PopReceipt { get; set; }
    public string Content { get; set; }
}
