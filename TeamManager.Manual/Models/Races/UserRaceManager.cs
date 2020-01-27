using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;
using System.IO;
using Microsoft.Extensions.Logging;

namespace TeamManager.Manual.Models
{
    public class UserRaceManager : IUserRaceManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly CustomUserManager userManager;
        private readonly IPointCalculator pointCalculator;
        private readonly ILogger<UserRaceManager> logger;
        private readonly IImageStore imageStore;
        public UserRaceManager(TeamManagerDbContext context, CustomUserManager customUserManager, IPointCalculator pointMgr, IImageStore imageStore, ILogger<UserRaceManager> userRaceManagerlogger)
        {
            dbContext = context;
            userManager = customUserManager;
            pointCalculator = pointMgr;
            this.imageStore = imageStore;
            logger = userRaceManagerlogger;
        }

        #region Entries

        public async Task AddEntryAsync(User user, RaceModel race)
        {
            if (user != null && race != null)
            {
                if (race.EntryDeadline.HasValue && race.EntryDeadline.Value <= DateTime.Now)
                {
                    logger.LogWarning($"Entry deadline is over for {race.Name}. User: {user.Email}");
                    throw new DeadlineException();
                }

                UserRace alreadyExistingEntity = dbContext.UserRaces.SingleOrDefault(ur => ur.RaceId == race.Id && ur.UserId == user.Id);
                if (alreadyExistingEntity != null)
                {
                    alreadyExistingEntity.IsEntryRequired = true;
                    dbContext.Entry<UserRace>(alreadyExistingEntity).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    UserRace userRace = new UserRace()
                    {
                        RaceId = race.Id,
                        UserId = user.Id,
                        IsEntryRequired = true
                    };

                    dbContext.UserRaces.Add(userRace);
                    await dbContext.SaveChangesAsync();
                }
                
                logger.LogInformation($"{user.Email} successfully required entry for {race.Name}");
            }
        }

        public async Task RemoveEntryAsync(User user, RaceModel race)
        {
            if (user != null && race != null)
            {
                if (race.EntryDeadline.HasValue && race.EntryDeadline.Value <= DateTime.Now)
                {
                    logger.LogWarning($"Entry deadline is over for {race.Name}. User: {user.Email}");
                    throw new DeadlineException();
                }

                UserRace alreadyExistingEntity = dbContext.UserRaces.SingleOrDefault(ur => ur.RaceId == race.Id && ur.UserId == user.Id);
                if (alreadyExistingEntity != null)
                {
                    dbContext.UserRaces.Remove(alreadyExistingEntity);
                    await dbContext.SaveChangesAsync();
                    logger.LogInformation($"{user.Email} successfully removed entry for {race.Name}");
                }
            }
        }

        public async Task<IList<UserModel>> ListEntriedUsersAsync(int id)
        {
            IEnumerable<int> userIds = dbContext.UserRaces.Where(x => x.RaceId == id && x.IsEntryRequired.HasValue && x.IsEntryRequired.Value).Select(x => x.UserId).ToList();
            if (userIds == null)
            {
                return new List<UserModel>();
            }

            var entriedUsers = await dbContext.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
            List<UserModel> response = new List<UserModel>();
            foreach (var user in entriedUsers)
            {
                response.Add(userManager.CreateUserModel(user));
            }

            logger.LogDebug($"{response.Count} entried riders returned for Race {id}");
            return response;
        }

        #endregion

        #region Results

        public async Task AddResultAsync(User user, int raceId, int? absoluteResult, int? categoryResult, bool? staff, IFormFile image)
        {
            UserRace userRace = dbContext.UserRaces.Include(x => x.Race).SingleOrDefault(x => x.RaceId == raceId && x.UserId == user.Id);
            bool update = userRace != null;
            if (!update)
            {
                userRace = new UserRace();
                userRace.UserId = user.Id;
                userRace.RaceId = raceId;
            }

            userRace.IsTakePartAsStaff = staff;
            userRace.CategoryResult = categoryResult;
            userRace.AbsoluteResult = absoluteResult;

            Race race = dbContext.Races.Find(raceId);
            if (user.IsPro)
                userRace.Points = 0;
            else
                userRace.Points = pointCalculator.CalculatePoints(race.PointWeight, race.OwnOrganizedEvent, userRace);

            if (image != null && image.Length > 0)
            {
                Uri imageUri = await UploadImage(user, image, userRace);
                if(imageUri != null)
                {
                    userRace.ImageUrl = imageUri.ToString();
                    userRace.ResultIsValid = true;
                }
            }
            
            if (update)
                dbContext.Entry<UserRace>(userRace).State = EntityState.Modified;
            else
                dbContext.UserRaces.Add(userRace);
            
            await dbContext.SaveChangesAsync();
            logger.LogInformation($"{user.Email} successfully added a new result for Race {raceId}");
        }

        private async Task<Uri> UploadImage(User user, IFormFile image, UserRace userRace)
        {
            Stream memoryStream = new MemoryStream();
            image.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return await imageStore.SaveRaceImageAsync(user, userRace.Race, memoryStream, image.ContentType);
        }

        public IList<ResultModel> GetRaceResultsByUser(User user)
        {
            IList<UserRace> racesOfTheGivenUser = dbContext.UserRaces
                .Include(x => x.Race)
                .Where(x => x.UserId == user.Id && (x.AbsoluteResult.HasValue || x.CategoryResult.HasValue || (x.IsTakePartAsStaff.HasValue && x.IsTakePartAsStaff.Value)))
                .ToList();
            return racesOfTheGivenUser.Select(x => ToResultModel(x)).ToList();
        }

        public async Task ChangeValidatedStatus(int userRaceId)
        {
            UserRace result = dbContext.UserRaces.Find(userRaceId);
            if (result == null)
            {
                throw new KeyNotFoundException(userRaceId.ToString());
            }

            result.ResultIsValid = !(result.ResultIsValid.HasValue && result.ResultIsValid.Value);
            dbContext.Entry(result).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation($"Status of result {result.Id} became {result.ResultIsValid.Value}");
        }

        private ResultModel ToResultModel(UserRace x)
        {
            return new ResultModel()
            {
                AbsoluteResult = x.AbsoluteResult,
                CategoryResult = x.CategoryResult,
                IsStaff = x.IsTakePartAsStaff.HasValue && x.IsTakePartAsStaff.Value,
                Points = x.Points,
                Race = x.Race.Name,
                RaceId = x.RaceId,
                UserId = x.UserId,
                RacePointWeight = x.Race.PointWeight,
                Image = x.ImageUrl,
                RaceDate = x.Race.Date,
                ResultId = x.Id,
                ResultIsValid = x.ResultIsValid
            };
        }

        #endregion
    }
}
