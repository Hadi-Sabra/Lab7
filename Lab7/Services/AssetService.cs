using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lab7.Services
{
    public class AssetService
    {
        private readonly string _assetsDirectory;

        public AssetService(IWebHostEnvironment env)
        {
            _assetsDirectory = Path.Combine(env.WebRootPath, "assets");

            // Ensure the directory exists
            if (!Directory.Exists(_assetsDirectory))
            {
                Directory.CreateDirectory(_assetsDirectory);
            }
        }

        // Upload a new file
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            string filePath = Path.Combine(_assetsDirectory, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/assets/{file.FileName}"; // Relative URL for accessing the file
        }

        // Get file path
        public string GetFilePath(string fileName)
        {
            string filePath = Path.Combine(_assetsDirectory, fileName);
            return File.Exists(filePath) ? filePath : null;
        }

        // Delete a file
        public bool DeleteFile(string fileName)
        {
            string filePath = Path.Combine(_assetsDirectory, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
    }
}