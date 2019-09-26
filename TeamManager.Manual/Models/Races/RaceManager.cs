﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class RaceManager : IRaceManager
    {
        private readonly TeamManagerDbContext dbContext;

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
            return ListRaces(dbContext.Races.ToList());
        }

        public IList<RaceModel> ListUpcomingRaces()
        {
            return ListRaces(dbContext.Races.Where(x => x.Date.HasValue && x.Date.Value > DateTime.Now).ToList());
        }

        public IList<RaceModel> ListPastRaces()
        {
            return ListRaces(dbContext.Races.Where(x => x.Date.HasValue && x.Date.Value < DateTime.Now).ToList());
        }

        private IList<RaceModel> ListRaces(IList<Race> from)
        {
            List<RaceModel> response = new List<RaceModel>();
            foreach (var race in from)
            {
                response.Add(ToRaceModel(race));
            }

            return response;
        }

        public RaceModel GetRaceById(int id)
        {
            Race raceWithTheGivenId = dbContext.Races.Find(id);
            if (raceWithTheGivenId == null)
            {
                return null;
            }

            return ToRaceModel(raceWithTheGivenId);
        }

        public async Task UpdateRaceAsync(RaceModel model)
        {
            Race raceToUpdate = dbContext.Races.Find(model.Id);
            if(raceToUpdate == null)
            {
                throw new KeyNotFoundException(model.Id.ToString());
            }

            raceToUpdate.City = model.City;
            raceToUpdate.Country = model.Country;
            raceToUpdate.Date = model.Date;
            raceToUpdate.EntryDeadline = model.EntryDeadline;
            raceToUpdate.Name = model.Name;
            raceToUpdate.PointWeight = model.PointWeight;
            raceToUpdate.Remark = model.Remark;
            raceToUpdate.TypeOfRace = model.TypeOfRace;
            raceToUpdate.Website = model.Website;
            raceToUpdate.OwnOrganizedEvent = model.OwnOrganizedEvent;

            // Add new distances
            foreach (var distance in model.DistanceLengths)
            {
                if(!dbContext.Distances.Any(x => x.RaceId == raceToUpdate.Id && x.Distance == distance))
                {
                    RaceDistance newDistance = new RaceDistance()
                    {
                        Distance = distance,
                        RaceId = raceToUpdate.Id
                    };

                    dbContext.Distances.Add(newDistance);
                }
            }

            // Remove not used distances
            var distances = dbContext.Distances.Where(x => x.RaceId == raceToUpdate.Id);
            foreach (var distance in distances)
            {
                if (!model.DistanceLengths.Contains(distance.Distance))
                {
                    dbContext.Distances.Remove(distance);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteRaceAsync(int id)
        {
            Race raceToUpdate = dbContext.Races.Find(id);
            if (raceToUpdate == null)
            {
                throw new KeyNotFoundException(id.ToString());
            }

            dbContext.Races.Remove(raceToUpdate);
            await dbContext.SaveChangesAsync();
        }

        private RaceModel ToRaceModel(Race r)
        {
            RaceModel raceModel = new RaceModel()
            {
                City = r.City,
                Country = r.Country,
                Date = r.Date,
                EntryDeadline = r.EntryDeadline,
                Id = r.Id,
                Name = r.Name,
                PointWeight = r.PointWeight,
                Remark = r.Remark,
                TypeOfRace = r.TypeOfRace,
                Website = r.Website,
                OwnOrganizedEvent = r.OwnOrganizedEvent
            };

            raceModel.DistanceLengths = dbContext.Distances.Where(x => x.RaceId == r.Id).Select(d => d.Distance).ToList();
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
                Website = raceModel.Website,
                OwnOrganizedEvent = raceModel.OwnOrganizedEvent
            };
        }
    }
}