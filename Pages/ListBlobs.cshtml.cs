using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ListBlobsModel : PageModel
{
    private readonly AzureBlobService _blobService;
    private readonly AuditLogService _auditLogService;

    public IEnumerable<BlobInfo> Blobs { get; private set; } = new List<BlobInfo>();

    public ListBlobsModel(AzureBlobService blobService, AuditLogService auditLogService)
    {
        _blobService = blobService;
        _auditLogService = auditLogService;
    }

    public async Task OnGetAsync()
    {
        Blobs = await _blobService.ListBlobsAsync("myfiles");
    }

    public async Task<IActionResult> OnPostDeleteAsync(string blobName)
    {
        if (string.IsNullOrEmpty(blobName))
        {
            return Page();
        }

        string userId = User.Identity?.Name ?? "Unknown";
        await _blobService.DeleteFileAsync("myfiles", blobName, userId);

        return RedirectToPage();
    }
}
