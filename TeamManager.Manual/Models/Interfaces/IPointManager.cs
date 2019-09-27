using System.Threading.Tasks;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IPointManager
    {
        int CalculatePoints(int raceWeight, bool ownEvent, int? categoryResult, bool? staff, bool? driver);

        Task AddConsumedPointAsync(string userId, int amount, string creatorUserId, string remark);
    }
}
