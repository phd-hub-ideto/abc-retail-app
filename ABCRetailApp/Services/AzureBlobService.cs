using Azure.Storage.Blobs;

namespace ABCRetailApp.Services;

public class AzureBlobService(
    IConfiguration configuration)
{
    private readonly string ContainerName = "cldv7112-productimages";

    public string GetContainerName() => ContainerName;

    public async Task UploadImageAsync(Stream imageStream, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString(Constants.AzureBlobStorage));
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(imageStream, overwrite: true);
    }

    public async Task<Stream> DownloadImageAsync(string fileName)
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString(Constants.AzureBlobStorage));
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var download = await blobClient.OpenReadAsync();

        return download;
    }

    public BlobContainerClient GetBlobContainerClient()
    {
        var blobServiceClient = new BlobServiceClient(configuration.GetConnectionString(Constants.AzureBlobStorage));

        return blobServiceClient.GetBlobContainerClient(ContainerName);
    }
}