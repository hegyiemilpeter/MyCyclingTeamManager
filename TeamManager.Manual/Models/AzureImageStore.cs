using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Diacritics.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class AzureImageStore : IImageStore
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AzureImageStore> logger;

        private string AzureConnectionString
        {
            get
            {
                return configuration.GetValue<string>("AzureBlobConnection");
            }
        }

        public AzureImageStore(IConfiguration config, ILogger<AzureImageStore> log)
        {
            configuration = config;
            logger = log;
        }

        public async Task<Uri> SaveRaceImageAsync(User user, Race race, Stream imageStream, string contentType)
        {
            try
            {
                if (string.IsNullOrEmpty(AzureConnectionString))
                {
                    logger.LogError($"No AzureBlobConnection is defined.");
                    return null;
                }

                string fileName = GenerateFileName(contentType);
                string blobContainerName = GenerateBlobContainerName(race);
                BlobContainerClient container = new BlobContainerClient(AzureConnectionString, blobContainerName);
                await container.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
                Response<BlobContentInfo> blob = await container.UploadBlobAsync(fileName, imageStream);
                logger.LogInformation($"Image {blobContainerName}/{fileName} saved to {race.Name} by {user.Email}");
                return GetBlobUri(blobContainerName, fileName);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Image cannot be saved to {race.Name} by {user.Email}");
                return null;
            }
        }

        private static string GenerateBlobContainerName(Race race)
        {
            return race.Date.Value.Year + "-" + race.Name.ToLower().Replace(" ", "-").RemoveDiacritics();
        }

        private static string GenerateFileName(string contentType)
        {
            string fileName = Guid.NewGuid().ToString();
            switch (contentType)
            {
                case "image/jpeg":
                    fileName += ".jpg";
                    break;
                case "image/png":
                    fileName += ".png";
                    break;
                default:
                    break;
            }

            return fileName;
        }

        public async Task<Stream> DownloadImageAsync(Uri imageUrl)
        {
            BlobClient blockBlobClient = new BlobClient(imageUrl);
            MemoryStream memoryStream = new MemoryStream();
            await blockBlobClient.DownloadToAsync(memoryStream);
            return memoryStream;
        }

        private Uri GetBlobUri(string containerName, string fileName)
        {
            BlockBlobClient blockBlobClient = new BlockBlobClient(AzureConnectionString, containerName, fileName);
            return blockBlobClient.Uri;
        }
    }
}
