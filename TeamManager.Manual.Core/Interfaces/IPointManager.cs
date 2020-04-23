using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IPointManager
    {
        Task<int> GetAvailablePointAmountByUser(string userId);

        Task<IList<PointConsuption>> ListConsumedPointsAsync(string userId);

        Task AddConsumedPointAsync(string userId, int amount, string creatorUserId, string remark);
    }
}
