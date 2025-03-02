using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Lab7.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration config)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = config["AzureStorage:stupefied_swartz"];

            // Ensure the container exists
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadProfilePictureAsync(string userId, IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient($"{userId}/{file.FileName}");

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString(); // Returns the URL of the uploaded file
        }

        public async Task<Stream?> DownloadProfilePictureAsync(string userId, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient($"{userId}/{fileName}");

            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            return null;
        }
    }
}