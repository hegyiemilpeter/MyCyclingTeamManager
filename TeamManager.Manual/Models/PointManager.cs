using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class PointManager : IPointManager
    {
        private static int STAFF_POINTS = 1;
        private static int ORGANIZER_POINTS = 5;
        private static int DRIVER_POINTS = 1;
        private static int TOP_10_BONUS = 1;
        private static int TOP_3_BONUS = 2;

        private readonly TeamManagerDbContext dbContext;
        private readonly CustomUserManager userManager;

        public PointManager(TeamManagerDbContext context, CustomUserManager customUserMgr)
        {
            dbContext = context;
            userManager = customUserMgr;
        }

        public async Task<IList<PointConsuption>> ListConsumedPointsAsync(string userId)
        {
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFoundException();
            }

            return dbContext.PointConsuptions.Where(x => x.UserId == user.Id).ToList();
        }

        public async Task AddConsumedPointAsync(string userId, int amount, string creatorUserId, string remark)
        {
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new UserNotFoundException();
            }

            User creator = await userManager.FindByIdAsync(creatorUserId);
            if(creator == null)
            {
                throw new UserNotFoundException();
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
        }

        public int CalculatePoints(int raceWeight, bool ownEvent, int? categoryResult, bool? staff, bool? driver)
        {
            int result = 0;
            if(categoryResult.HasValue && categoryResult.Value > 0)
            {
                if(categoryResult.Value <= 3)
                {
                    result += TOP_3_BONUS;
                }
                else if(categoryResult.Value <= 10)
                {
                    result += TOP_10_BONUS;
                }

                result += raceWeight;
            }

            if(staff.HasValue && staff.Value)
            {
                if (ownEvent)
                {
                    result += ORGANIZER_POINTS;
                }
                else
                {
                    result += STAFF_POINTS;
                }
            }

            if(driver.HasValue && driver.Value)
            {
                result += DRIVER_POINTS;
            }

            return result;
        }
    }
}
