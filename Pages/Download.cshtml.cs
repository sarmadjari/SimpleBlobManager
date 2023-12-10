using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class DownloadModel : PageModel
{
    private readonly AzureBlobService _blobService;

    public DownloadModel(AzureBlobService blobService)
    {
        _blobService = blobService;
    }

    public async Task<IActionResult> OnGetAsync(string containerName, string blobName)
    {
        if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
        {
            return BadRequest();
        }

        try
        {
            var stream = await _blobService.GetBlobStreamAsync(containerName, blobName);
            return File(stream, "application/octet-stream", blobName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}
