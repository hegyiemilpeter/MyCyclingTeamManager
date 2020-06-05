using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Services
{
    public class RaceResultManager : IRaceResultManager
    {
        public UnitOfWork UnitOfWork { get; }
        public IConfiguration Configuration { get; }
        public ILogger<RaceResultManager> Logger { get; }
        public IPointCalculator PointCalculator { get; }
        public IImageStore ImageStore { get; }

        public RaceResultManager(TeamManagerDbContext dbContext, IConfiguration configuration, IPointCalculator pointCalculator, IImageStore imageStore, ILogger<RaceResultManager> logger) : this(new UnitOfWork(dbContext), configuration, pointCalculator, imageStore, logger)
        {
        }

        protected internal RaceResultManager(UnitOfWork unitOfWork, IConfiguration configuration, IPointCalculator pointCalculator, IImageStore imageStore, ILogger<RaceResultManager> logger)
        {
            UnitOfWork = unitOfWork;
            Configuration = configuration;
            PointCalculator = pointCalculator;
            ImageStore = imageStore;
            Logger = logger;
        }

        public async Task AddResultAsync(int userId, int raceId, int? absoluteResult, int? categoryResult, bool? staff, MemoryStream image, string contentTypeForImage)
        {
            UserRace userRace = new UserRace
            {
                UserId = userId,
                RaceId = raceId,
                IsTakePartAsStaff = staff,
                CategoryResult = categoryResult,
                AbsoluteResult = absoluteResult
            };

            Race race = await UnitOfWork.RaceRepository.GetByIDAsync(raceId);

            DateTime deadline = race.Date.Value.AddDays(7);
            int deadlineDaysForPointConsuption = Configuration.GetValue<int>("ResultAddDeadline");
            if (deadlineDaysForPointConsuption > 0)
            {
                deadline = new DateTime(race.Date.Value.Year, race.Date.Value.Month, race.Date.Value.AddDays(deadlineDaysForPointConsuption).Day, 23, 59, 59);
            }

            User user = await UnitOfWork.UserRepository.GetByIDAsync(userId);
            userRace.Points = PointCalculator.CalculatePoints(user.IsPro, race.PointWeight, race.OwnOrganizedEvent, deadline, userRace);

            if (image != null && image.Length > 0)
            {
                Uri imageUri = await ImageStore.SaveRaceImageAsync(user, race, image, contentTypeForImage);
                if (imageUri != null)
                {
                    userRace.ImageUrl = imageUri.ToString();
                    userRace.ResultIsValid = true;
                }
            }

            await UnitOfWork.UserRaceRepository.CreateAsync(userRace);
            UnitOfWork.Save();

            Logger.LogInformation($"'{user.Email}' successfully added a new result for Race '{race.Name}'");
        }

        public IEnumerable<ResultModel> ListRaceResultsByUserId(int userId)
        {
            return UnitOfWork.UserRaceRepository.ListUserRacesByUserId(userId).Select(x => new ResultModel
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
            });
        }

        public async Task ChangeValidatedStatus(int userRaceId)
        {
            UserRace result = await UnitOfWork.UserRaceRepository.GetByIDAsync(userRaceId);
            if (result == null)
            {
                throw new KeyNotFoundException(userRaceId.ToString());
            }

            result.ResultIsValid = !(result.ResultIsValid.HasValue && result.ResultIsValid.Value);
            throw new NotImplementedException("Point re-calculation is needed when a result is updated.");

            await UnitOfWork.UserRaceRepository.UpdateAsync(result);
            UnitOfWork.Save();

            Logger.LogInformation($"Status of result {result.Id} became {result.ResultIsValid.Value}");
        }
    }
}
