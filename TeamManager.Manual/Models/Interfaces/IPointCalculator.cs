using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IPointCalculator
    {
        int CalculatePoints(int raceWeight, bool ownEvent, UserRace result);
    }
}
