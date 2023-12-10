using Azure.Data.Tables;
using Azure.Identity;
using System;
using System.Threading.Tasks;

public class AuditLogService
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly string _tableName = "AuditLogs";

        public AuditLogService(string tableEndpoint)
    {
        // Use DefaultAzureCredential for Managed Identity
        _tableServiceClient = new TableServiceClient(new Uri(tableEndpoint), new DefaultAzureCredential());
        _tableServiceClient.CreateTableIfNotExists(_tableName);
    }

    public async Task LogActivityAsync(string userId, string action, string details)
    {
        var tableClient = _tableServiceClient.GetTableClient(_tableName);
        var logEntry = new TableEntity(Guid.NewGuid().ToString(), DateTime.UtcNow.ToString("o"))
        {
            { "UserId", userId },
            { "Action", action },
            { "Details", details },
            { "Timestamp", DateTime.UtcNow }
        };
        await tableClient.AddEntityAsync(logEntry);
    }
}
