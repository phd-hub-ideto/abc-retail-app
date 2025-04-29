using ABCRetailApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<AzureTableService>();
        services.AddSingleton<AzureBlobService>();
        services.AddSingleton<AzureQueueStorageService>();
        services.AddSingleton<AzureFileStorageService>();
    })
    .Build();

host.Run();