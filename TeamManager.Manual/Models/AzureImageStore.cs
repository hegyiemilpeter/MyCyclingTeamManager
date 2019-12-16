using Azure.Storage.Blobs;
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

        public AzureImageStore(IConfiguration config, ILogger<AzureImageStore> log)
        {
            configuration = config;
            logger = log;
        }

        public async Task SaveRaceImageAsync(User user, Stream imageStream, string fileName, Race race)
        {
            try
            {
                string azureConnectionString = configuration.GetValue<string>("AzureBlobConnection");
                string blobName = race.Date.Value.Year + "-" + race.Name.ToLower().Replace(" ", "-").RemoveDiacritics();
                if (!string.IsNullOrEmpty(azureConnectionString))
                {
                    BlobContainerClient container = new BlobContainerClient(azureConnectionString, blobName);
                    await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                    await container.UploadBlobAsync(fileName, imageStream);
                }

                logger.LogInformation($"Image {blobName}/{fileName} saved to {race.Name} by {user.Email}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Image cannot be saved to {race.Name} by {user.Email}");
            }
        }
    }
}
