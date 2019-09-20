using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
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

            if(raceModel.DistanceLengths != null)
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
                .Select(r => GetRaceModel(r))
                .ToList();
        }

        private static RaceModel GetRaceModel(Race r)
        {
            RaceModel raceModel = ToRaceModel(r);
            raceModel.DistanceLengths = raceModel.Distances.Select(d => d.Distance).ToList();
            return raceModel;
        }

        private static RaceModel ToRaceModel(Race race)
        {
            return new RaceModel()
            {
                City = race.City,
                Country = race.Country,
                Date = race.Date,
                Distances = race.Distances,
                EntryDeadline = race.EntryDeadline,
                Id = race.Id,
                Name = race.Name,
                PointWeight = race.PointWeight,
                Remark = race.Remark,
                TypeOfRace = race.TypeOfRace,
                Website = race.Website
            };
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
