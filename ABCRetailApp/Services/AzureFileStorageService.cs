using ABCRetailApp.Models;
using Azure;
using Azure.Storage.Files.Shares;

namespace ABCRetailApp.Services;

public class AzureFileStorageService
{
    private readonly ShareClient _shareClient;
    private readonly ShareDirectoryClient _directoryClient;

    public AzureFileStorageService(IConfiguration configuration)
    {
        var shareServiceClient = new ShareServiceClient(configuration.GetConnectionString(Constants.AzureBlobStorage));
        _shareClient = shareServiceClient.GetShareClient("contracts");
        _shareClient.CreateIfNotExists();
        _directoryClient = _shareClient.GetRootDirectoryClient();
    }

    public async Task UploadFileAsync(string fileName, Stream stream)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);
        await fileClient.CreateAsync(stream.Length);
        await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
    }

    public async Task<IEnumerable<FileShareItem>> ListFilesAsync()
    {
        var files = new List<FileShareItem>();
        await foreach (var fileItem in _directoryClient.GetFilesAndDirectoriesAsync())
        {
            files.Add(new FileShareItem { FileName = fileItem.Name, FileUrl = _directoryClient.GetFileClient(fileItem.Name).Uri.ToString() });
        }
        return files;
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);
        var downloadResponse = await fileClient.DownloadAsync();
        return downloadResponse.Value.Content;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);
        await fileClient.DeleteIfExistsAsync();
    }

    public async Task<bool> FileExistsAsync(string fileName)
    {
        var fileClient = _directoryClient.GetFileClient(fileName);
        var response = await fileClient.ExistsAsync();
        return response.Value;
    }
}