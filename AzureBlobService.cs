using Azure.Storage.Blobs;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class AzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AuditLogService _auditLogService;

    public AzureBlobService(string blobServiceEndpoint, AuditLogService auditLogService)
    {
        _blobServiceClient = new BlobServiceClient(new Uri(blobServiceEndpoint), new DefaultAzureCredential());
        _auditLogService = auditLogService;
    }

    public async Task<IEnumerable<BlobInfo>> ListBlobsAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobs = new List<BlobInfo>();

        await foreach (var blobItem in containerClient.GetBlobsAsync())
        {
            blobs.Add(new BlobInfo { Name = blobItem.Name, Url = containerClient.GetBlobClient(blobItem.Name).Uri.ToString() });
        }

        return blobs;
    }

    public async Task<Stream> GetBlobStreamAsync(string containerName, string blobName)
    {
        var blobClient = _blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);
        if (await blobClient.ExistsAsync())
        {
            return await blobClient.OpenReadAsync();
        }
        else
        {
            throw new FileNotFoundException("Blob not found.", blobName);
        }
    }

    public async Task UploadFileAsync(string containerName, string fileName, Stream content, string userId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(content, overwrite: true);

        // Audit logging
        await _auditLogService.LogActivityAsync(userId, "Upload", $"Uploaded file: {fileName} to container: {containerName}");
    }

    public async Task DeleteFileAsync(string containerName, string fileName, string userId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();

        // Audit logging
        await _auditLogService.LogActivityAsync(userId, "Delete", $"Deleted file: {fileName} from container: {containerName}");
    }


}

public class BlobInfo
{
    public string Name { get; set; }
    public string Url { get; set; }

    public BlobInfo()
    {
        Name = string.Empty;
        Url = string.Empty;
    }
}

