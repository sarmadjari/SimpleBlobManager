using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

[Authorize]
public class UploadModel : PageModel
{
    [BindProperty]
    [Required]
    public IFormFile? FileUpload { get; set; }

    private readonly AzureBlobService _blobService;
    private readonly AuditLogService _auditLogService;

    public UploadModel(AzureBlobService blobService, AuditLogService auditLogService)
    {
        _blobService = blobService;
        _auditLogService = auditLogService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (FileUpload != null)
        {
            using var stream = new MemoryStream();
            await FileUpload.CopyToAsync(stream);
            stream.Position = 0;
            string userId = User.Identity?.Name ?? "Unknown";
            await _blobService.UploadFileAsync("myfiles", FileUpload.FileName, stream, userId);
        }

        return RedirectToPage("./Index");
    }
}
