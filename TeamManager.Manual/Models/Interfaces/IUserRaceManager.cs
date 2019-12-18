using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IUserRaceManager
    {
        Task AddEntryAsync(User user, RaceModel race);

        Task RemoveEntryAsync(User user, RaceModel race);

        Task<IList<UserModel>> ListEntriedUsersAsync(int id);

        Task AddResultAsync(User user, int raceId, int? absoluteResult, int? categoryResult, bool? driver, bool? staff, IFormFile image);

        IList<ResultModel> GetRaceResultsByUser(User user);

        Task ChangeValidatedStatus(int userRaceId);
    }
}
