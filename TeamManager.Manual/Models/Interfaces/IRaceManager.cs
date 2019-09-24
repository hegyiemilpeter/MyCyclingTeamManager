using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IRaceManager
    {
        Task AddRaceAsync(RaceModel raceModel);

        Task UpdateRaceAsync(RaceModel model);

        Task DeleteRaceAsync(int id);

        IList<RaceModel> ListRaces();

        RaceModel GetRaceById(int id);

        Task AddEntryAsync(User user, Race race);

        Task RemoveEntryAsync(User user, Race race);

        Task<IList<User>> ListEntriedUsersAsync(int id);
    }
}
