using System.Collections.Generic;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IPointManager
    {
        Task<int> GetAvailablePointAmountByUser(int userId);

        Task<IList<PointConsuption>> ListConsumedPointsAsync(int userId);

        Task AddConsumedPointAsync(int userId, int amount, string creatorUserId, string remark);
    }
}
