using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class BillManager : IBillManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly ILogger<BillManager> logger;
        private readonly IImageStore imageStore;
        private readonly IPointCalculator pointCalculator;
        private readonly IEmailSender emailSender;

        public BillManager(TeamManagerDbContext context, ILogger<BillManager> bmLogger, IImageStore imgStore, IPointCalculator pointCalc, IEmailSender mailSender)
        {
            dbContext = context;
            logger = bmLogger;
            imageStore = imgStore;
            pointCalculator = pointCalc;
            emailSender = mailSender;
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
                UserId = user.Id,
                Points = pointCalculator.CalculatePoints(amount)
            };

            if (billUri != null)
            {
                newBill.Url = billUri.ToString();
            }

            dbContext.Add(newBill);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"A new bill created to {user.Email}. Id: {newBill.Id}");
        }

        public async Task DeleteBillAsync(int billId)
        {
            Bill billToDelete = dbContext.Bills.Include(x => x.User).SingleOrDefault(x => x.Id == billId);
            if(billToDelete != null)
            {
                dbContext.Bills.Remove(billToDelete);
                await dbContext.SaveChangesAsync();
                await emailSender.SendBillDeletedEmailAsync(billToDelete.User.Email, billToDelete.User.FirstName, billToDelete.Amount, billToDelete.PurchaseDate, billToDelete.Url);
            }
        }

        public async Task<Bill> GetBillByIdAsync(int billId)
        {
            return await dbContext.FindAsync<Bill>(billId);
        }

        public async Task<IList<Bill>> ListBillsAsync()
        {
            return await dbContext.Bills.Include(x => x.User).ToListAsync();
        }

        public async Task<BillModel> ListBillsByUserAsync(int userId)
        {
            BillModel response = new BillModel();
            response.Bills = await dbContext.Bills.Where(x => x.UserId == userId).ToListAsync();
            response.Points = response.Bills.Sum(x => x.Points);
            return response;
        }
    }
}
