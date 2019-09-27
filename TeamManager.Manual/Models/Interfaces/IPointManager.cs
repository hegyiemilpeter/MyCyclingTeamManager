using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IPointManager
    {
        int CalculatePoints(int raceWeight, bool ownEvent, int? categoryResult, bool? staff, bool? driver);
        
        Task<IList<PointConsuption>> ListConsumedPointsAsync(string userId);

        Task AddConsumedPointAsync(string userId, int amount, string creatorUserId, string remark);
    }
}
