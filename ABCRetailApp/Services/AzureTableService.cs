using ABCRetailApp.Models;
using Azure.Data.Tables;

namespace ABCRetailApp.Services;

public class AzureTableService(
    IConfiguration configuration)
{
    private readonly string CustomerTableName = "CustomerProfiles";
    private readonly string ProductTableName = "Products";

    public async Task<IEnumerable<CustomerProfile>> GetAllCustomerProfilesAsync()
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), CustomerTableName);
        var entities = client.QueryAsync<CustomerProfile>();

        var customerList = new List<CustomerProfile>();

        // Iterate through all pages and add to the list
        await foreach (var entity in entities)
        {
            customerList.Add(entity);
        }

        return customerList;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), ProductTableName);
        var entities = client.QueryAsync<Product>();
        
        var productList = new List<Product>();

        // Iterate through all pages and add to the list
        await foreach (var entity in entities)
        {
            productList.Add(entity);
        }

        return productList;
    }

    public async Task AddCustomerProfileAsync(CustomerProfile profile)
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), CustomerTableName);
        await client.CreateIfNotExistsAsync();
        await client.AddEntityAsync(profile);
    }

    public async Task<CustomerProfile> GetCustomerProfileAsync(string partitionKey, string rowKey)
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), CustomerTableName);
        var entity = await client.GetEntityAsync<CustomerProfile>(partitionKey, rowKey);
        return entity.Value;
    }

    public async Task AddProductAsync(Product product)
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), ProductTableName);
        await client.CreateIfNotExistsAsync();
        await client.AddEntityAsync(product);
    }

    public async Task<Product> GetProductAsync(string partitionKey, string rowKey)
    {
        var client = new TableClient(configuration.GetConnectionString(Constants.AzureBlobStorage), ProductTableName);
        var entity = await client.GetEntityAsync<Product>(partitionKey, rowKey);
        return entity.Value;
    }
}