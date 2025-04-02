using Azure;
using Azure.Data.Tables;

namespace ABCRetailApp.Models;

public class CustomerProfile : ITableEntity
{
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public string PartitionKey { get; set; }  // Usually a common category (e.g., 'customer')
    public string RowKey { get; set; }        // Unique identifier for each entity (e.g., customer ID)
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
}