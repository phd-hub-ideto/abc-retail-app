using System.Text.Json;

namespace ABCRetailApp;

public static class Constants
{
    public static string AzureBlobStorage = "AzureBlobStorage";
    public static string FunctionsBaseUrl = "FunctionsBaseUrl";
    public static JsonSerializerOptions SharedJsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
}
