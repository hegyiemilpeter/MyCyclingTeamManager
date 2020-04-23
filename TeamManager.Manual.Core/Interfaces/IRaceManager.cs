using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IRaceManager
    {
        Task AddRaceAsync(RaceModel race);

        Task UpdateRaceAsync(RaceModel race);

        Task DeleteRaceAsync(int raceId);

        IList<RaceModel> ListRaces();

        IList<RaceModel> ListUpcomingRaces();

        IList<RaceModel> ListRacesForResultAdd();

        RaceModel GetRaceById(int raceId);
    }
}
