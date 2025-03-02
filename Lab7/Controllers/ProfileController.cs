using Lab7.Data;
using Lab7.Models;
using Lab7.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab7.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly AppDbContext _dbContext;

        public ProfileController(BlobStorageService blobStorageService, AppDbContext dbContext)
        {
            _blobStorageService = blobStorageService;
            _dbContext = dbContext;
        }

        [Authorize] // Requires authentication
        [HttpPost("upload")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Get user ID from claims
            var userId = User.Identity?.Name ?? "anonymous"; // Fallback if no username is found

            var url = await _blobStorageService.UploadProfilePictureAsync(userId, file);

            // Update user profile in DB
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == userId);
            if (user != null)
            {
                user.ProfilePictureUrl = url;
                _dbContext.SaveChanges();
            }

            return Ok(new { ImageUrl = url });
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadProfilePicture(string fileName)
        {
            var userId = User.Identity?.Name ?? "anonymous";

            var stream = await _blobStorageService.DownloadProfilePictureAsync(userId, fileName);
            if (stream == null)
                return NotFound("File not found.");

            return File(stream, "image/jpeg");
        }
    }
}