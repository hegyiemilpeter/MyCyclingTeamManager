using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Models
{
    public class RaceManager : IRaceManager
    {
        private TeamManagerDbContext dbContext { get; }

        public RaceManager(TeamManagerDbContext context)
        {
            dbContext = context;
        }

        public async Task AddRaceAsync(RaceModel raceModel)
        {
            Race race = ToRace(raceModel);
            dbContext.Races.Add(race);
            await dbContext.SaveChangesAsync();

            if (raceModel.DistanceLengths != null)
            {
                foreach (var length in raceModel.DistanceLengths)
                {
                    RaceDistance distance = new RaceDistance();
                    distance.Distance = length;
                    distance.RaceId = race.Id;
                    dbContext.Distances.Add(distance);
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public IList<RaceModel> ListRaces()
        {
            return dbContext.Races
                .Include(r => r.Distances)
                .Select(r => ToRaceModel(r))
                .ToList();
        }

        public RaceModel GetById(int id)
        {
            Race raceWithTheGivenId = dbContext.Races.Include(r => r.Distances).FirstOrDefault(r => r.Id == id);
            if (raceWithTheGivenId == null)
            {
                return null;
            }

            return ToRaceModel(raceWithTheGivenId);
        }

        public async Task<IList<User>> ListEntriedUsersAsync(int id)
        {
            IEnumerable<int> userIds = dbContext.UserRaces.Where(x => x.RaceId == id && x.IsEntryRequired.HasValue && x.IsEntryRequired.Value).Select(x => x.UserId).ToList();
            if (userIds == null)
            {
                return new List<User>();
            }

            return await dbContext.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
        }

        public async Task AddEntryAsync(User user, Race race)
        {
            if (user != null && race != null)
            {
                if (race.EntryDeadline.HasValue && race.EntryDeadline.Value <= DateTime.Now)
                {
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
            }
        }

        public async Task RemoveEntryAsync(User user, Race race)
        {
            if (user != null && race != null)
            {
                if (race.EntryDeadline.HasValue && race.EntryDeadline.Value <= DateTime.Now)
                {
                    throw new DeadlineException();
                }

                UserRace alreadyExistingEntity = dbContext.UserRaces.SingleOrDefault(ur => ur.RaceId == race.Id && ur.UserId == user.Id);
                if(alreadyExistingEntity != null)
                {
                    alreadyExistingEntity.IsEntryRequired = false;
                    dbContext.Entry<UserRace>(alreadyExistingEntity).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        

        private static RaceModel ToRaceModel(Race r)
        {
            RaceModel raceModel = new RaceModel()
            {
                City = r.City,
                Country = r.Country,
                Date = r.Date,
                Distances = r.Distances,
                EntryDeadline = r.EntryDeadline,
                Id = r.Id,
                Name = r.Name,
                PointWeight = r.PointWeight,
                Remark = r.Remark,
                TypeOfRace = r.TypeOfRace,
                Website = r.Website
            };

            raceModel.DistanceLengths = raceModel.Distances.Select(d => d.Distance).ToList();
            return raceModel;
        }

        private static Race ToRace(RaceModel raceModel)
        {
            return new Race()
            {
                City = raceModel.City,
                Country = raceModel.Country,
                Date = raceModel.Date,
                EntryDeadline = raceModel.EntryDeadline,
                Id = raceModel.Id,
                Name = raceModel.Name,
                PointWeight = raceModel.PointWeight,
                Remark = raceModel.Remark,
                TypeOfRace = raceModel.TypeOfRace,
                Website = raceModel.Website
            };
        }
    }
}
