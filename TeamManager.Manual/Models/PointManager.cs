using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;
using Microsoft.Extensions.Logging;

namespace TeamManager.Manual.Models
{
    public class PointManager : IPointManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly CustomUserManager userManager;
        private readonly IUserRaceManager userRaceManager;
        private readonly ILogger<PointManager> logger;

        public PointManager(TeamManagerDbContext context, CustomUserManager customUserMgr, IUserRaceManager userRaceMgr, ILogger<PointManager> pmLogger)
        {
            dbContext = context;
            userManager = customUserMgr;
            userRaceManager = userRaceMgr;
            logger = pmLogger;
        }

        public async Task<int> GetAvailablePointAmountByUser(string userId)
        {
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                logger.LogWarning($"User with id {userId} is not found");
                throw new UserNotFoundException();
            }

            int gainedPoints = userRaceManager.GetRaceResultsByUser(user).Sum(x => x.Points);
            int consumedPoints = (await ListConsumedPointsAsync(userId)).Sum(x => x.Amount);
            return gainedPoints - consumedPoints;
        }

        public async Task<IList<PointConsuption>> ListConsumedPointsAsync(string userId)
        {
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                logger.LogWarning($"User with id {userId} is not found");
                throw new UserNotFoundException();
            }

            return dbContext.PointConsuptions.Where(x => x.UserId == user.Id).ToList();
        }

        public async Task AddConsumedPointAsync(string userId, int amount, string creatorUserId, string remark)
        {
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                logger.LogWarning($"User with id {userId} is not found");
                throw new UserNotFoundException();
            }

            User creator = await userManager.FindByIdAsync(creatorUserId);
            if(creator == null)
            {
                logger.LogWarning($"User with id {creatorUserId} is not found");
                throw new UserNotFoundException();
            }

            int currentAmount = await GetAvailablePointAmountByUser(user.Id.ToString());
            if(currentAmount < amount)
            {
                logger.LogError($"User {creatorUserId} try to overdue to point limit of user {userId}");
                throw new PointLimitException();
            }

            PointConsuption consuption = new PointConsuption()
            {
                Amount = amount,
                CreatedAt = DateTime.Now,
                CreatedBy = creator.FirstName + " " + creator.LastName,
                UserId = user.Id,
                Remark = remark
            };

            dbContext.PointConsuptions.Add(consuption);
            await dbContext.SaveChangesAsync();
            logger.LogInformation($"{amount} points are consumed by user {userId}. Created by {creatorUserId} - {remark}");
        }
    }
}
