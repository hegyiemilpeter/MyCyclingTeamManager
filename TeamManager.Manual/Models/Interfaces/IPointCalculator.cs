namespace TeamManager.Manual.Models.Interfaces
{
    public interface IPointCalculator
    {
        int CalculatePoints(int raceWeight, bool ownEvent, int? categoryResult, bool? staff, bool? driver);
    }
}
