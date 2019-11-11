﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;
using Azure.Storage;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diacritics.Extensions;
using System.IO;

namespace TeamManager.Manual.Models
{
    public class UserRaceManager : IUserRaceManager
    {
        private readonly TeamManagerDbContext dbContext;
        private readonly CustomUserManager userManager;
        private readonly IPointCalculator pointCalculator;
        private readonly IConfiguration configuration;
        public UserRaceManager(TeamManagerDbContext context, CustomUserManager customUserManager, IPointCalculator pointMgr, IConfiguration config)
        {
            dbContext = context;
            userManager = customUserManager;
            pointCalculator = pointMgr;
            configuration = config;
        }

        #region Entries

        public async Task AddEntryAsync(User user, RaceModel race)
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

        public async Task RemoveEntryAsync(User user, RaceModel race)
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
                    dbContext.UserRaces.Remove(alreadyExistingEntity);
                    await dbContext.SaveChangesAsync();
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

            var entriedUsers = dbContext.Users.Where(x => userIds.Contains(x.Id)).ToList();
            List<UserModel> response = new List<UserModel>();
            foreach (var user in entriedUsers)
            {
                response.Add(userManager.CreateUserModel(user));
            }

            return response;
        }

        #endregion

        #region Results

        public async Task AddResultAsync(User user, int raceId, int? absoluteResult, int? categoryResult, bool? driver, bool? staff, IFormFile image)
        {
            UserRace userRace = dbContext.UserRaces.Include(x => x.Race).SingleOrDefault(x => x.RaceId == raceId && x.UserId == user.Id);
            bool update = userRace != null;
            if (!update)
            {
                userRace = new UserRace();
                userRace.UserId = user.Id;
                userRace.RaceId = raceId;
            }

            userRace.IsTakePartAsDriver = driver;
            userRace.IsTakePartAsStaff = staff;
            userRace.CategoryResult = categoryResult;
            userRace.AbsoluteResult = absoluteResult;

            if (update)
            {
                dbContext.Entry<UserRace>(userRace).State = EntityState.Modified;
            }
            else
            {
                dbContext.UserRaces.Add(userRace);
            }

            try
            {
                string azureConnectionString = configuration.GetValue<string>("AzureBlobConnection");
                if (!string.IsNullOrEmpty(azureConnectionString))
                {
                    string blobName = userRace.Race.Date.Value.Year + "-" + userRace.Race.Name.ToLower().Replace(" ", "-").RemoveDiacritics();
                    BlobContainerClient container = new BlobContainerClient(azureConnectionString, blobName);
                    await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                    Stream memoryStream = new MemoryStream();
                    image.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    await container.UploadBlobAsync(image.FileName, memoryStream);
                }
            }
            catch (Exception)
            {
                // TODO: log errors here
            }

            await dbContext.SaveChangesAsync();
        }

        public IList<ResultModel> GetRaceResultsByUser(User user)
        {
            IList<UserRace> releavantRaces = dbContext.UserRaces
                .Include(x => x.Race)
                .Where(x => x.UserId == user.Id && (x.AbsoluteResult.HasValue || x.CategoryResult.HasValue || (x.IsTakePartAsDriver.HasValue && x.IsTakePartAsDriver.Value) || (x.IsTakePartAsStaff.HasValue && x.IsTakePartAsStaff.Value)))
                .ToList();
            return releavantRaces.Select(x => ToResultModel(x)).ToList();
        }

        private ResultModel ToResultModel(UserRace x)
        {
            return new ResultModel()
            {
                AbsoluteResult = x.AbsoluteResult,
                CategoryResult = x.CategoryResult,
                IsDriver = x.IsTakePartAsDriver.HasValue && x.IsTakePartAsDriver.Value,
                IsStaff = x.IsTakePartAsStaff.HasValue && x.IsTakePartAsStaff.Value,
                Points = pointCalculator.CalculatePoints(x.Race.PointWeight, x.Race.OwnOrganizedEvent, x.CategoryResult, x.IsTakePartAsStaff, x.IsTakePartAsDriver),
                Race = x.Race.Name,
                RaceId = x.RaceId,
                UserId = x.UserId,
                RacePointWeight = x.Race.PointWeight
            };
        }

        #endregion
    }
}
