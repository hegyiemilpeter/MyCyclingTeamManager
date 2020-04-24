using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IUserRaceManager
    {
        Task AddEntryAsync(User user, Race race);

        Task RemoveEntryAsync(User user, Race race);

        Task<IList<string>> ListEntriedUsersAsync(int id);

        Task AddResultAsync(User user, int raceId, int? absoluteResult, int? categoryResult, bool? staff, IFormFile image);

        IList<ResultModel> GetRaceResultsByUser(User user);

        Task ChangeValidatedStatus(int userRaceId);
    }
}
