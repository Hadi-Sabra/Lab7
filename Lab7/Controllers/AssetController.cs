using Lab7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Lab7.Controllers
{
    [Route("api/assets")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly AssetService _assetService;

        public AssetController(AssetService assetService)
        {
            _assetService = assetService;
        }

        // Upload a file
        [HttpPost("upload")]
        [Authorize(Policy = "TeacherOnly")] // Only teachers can upload assets
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            string fileUrl = await _assetService.UploadFileAsync(file);
            return Ok(new { message = "File uploaded successfully", url = fileUrl });
        }

        // Get a file (download)
        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            string filePath = _assetService.GetFilePath(fileName);

            if (filePath == null)
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream"; // Default content type
            return File(fileBytes, contentType, fileName);
        }

        // Delete a file
        [HttpDelete("{fileName}")]
        [Authorize(Policy = "TeacherOnly")] // Only teachers can delete assets
        public IActionResult DeleteFile(string fileName)
        {
            bool deleted = _assetService.DeleteFile(fileName);

            if (!deleted)
                return NotFound("File not found.");

            return Ok(new { message = "File deleted successfully" });
        }
    }
}