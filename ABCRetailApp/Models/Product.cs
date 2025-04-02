using Azure;
using Azure.Data.Tables;

namespace ABCRetailApp.Models;

public class Product : ITableEntity
{
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string PartitionKey { get; set; }  // Typically 'product' or category of product
    public string RowKey { get; set; }        // Product unique identifier (e.g., product ID)
    public string Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public string ImageUrl { get; set; }
}