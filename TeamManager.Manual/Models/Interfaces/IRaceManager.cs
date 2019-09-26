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
        Task AddRaceAsync(RaceModel race);

        Task UpdateRaceAsync(RaceModel race);

        Task DeleteRaceAsync(int raceId);

        IList<RaceModel> ListRaces();

        IList<RaceModel> ListUpcomingRaces();

        IList<RaceModel> ListPastRaces();

        RaceModel GetRaceById(int raceId);
    }
}
