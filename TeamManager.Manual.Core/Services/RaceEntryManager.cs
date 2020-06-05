using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Core.Interfaces;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Exceptions;

namespace TeamManager.Manual.Core.Services
{
    public class RaceEntryManager : IRaceEntryManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly ILogger<RaceEntryManager> logger;
        public RaceEntryManager(TeamManagerDbContext context,  ILogger<RaceEntryManager> userRaceManagerlogger)
        {
            dbContext = context;
            logger = userRaceManagerlogger;
        }

        public async Task AddEntryAsync(User user, Race race)
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

        public async Task RemoveEntryAsync(User user, Race race)
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

        public async Task<IList<string>> ListEntriedUsersAsync(int id)
        {
            IEnumerable<int> userIds = dbContext.UserRaces.Where(x => x.RaceId == id && x.IsEntryRequired.HasValue && x.IsEntryRequired.Value).Select(x => x.UserId).ToList();
            if (userIds == null)
            {
                return new List<string>();
            }

            IList<string> entriedUsers = await dbContext.Users.Where(x => userIds.Contains(x.Id)).Select(x => $"{x.FirstName} {x.LastName}").ToListAsync();
            logger.LogDebug($"{entriedUsers.Count} entried riders returned for Race {id}");
            return entriedUsers;
        }
    }
}
