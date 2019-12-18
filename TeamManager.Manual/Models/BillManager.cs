using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class BillManager : IBillManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly ILogger<BillManager> logger;
        private readonly IImageStore imageStore;

        public BillManager(TeamManagerDbContext context, ILogger<BillManager> bmLogger, IImageStore imgStore)
        {
            dbContext = context;
            logger = bmLogger;
            imageStore = imgStore;
        }

        public async Task CreateBillAsync(User user, int amount, DateTime createdAt, IFormFile image)
        {
            Stream memoryStream = new MemoryStream();
            image.CopyTo(memoryStream);
            memoryStream.Position = 0;

            Uri billUri = await imageStore.SaveBillImageAsync(createdAt, memoryStream, image.ContentType);

            Bill newBill = new Bill()
            {
                Amount = amount,
                PurchaseDate = createdAt,
                UserId = user.Id
            };

            if (billUri == null)
            {
                newBill.Url = billUri.ToString();
            }

            dbContext.Add(newBill);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"A new bill created to {user.Email}. Id: {newBill.Id}");
        }
    }
}
