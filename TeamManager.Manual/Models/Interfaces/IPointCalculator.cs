using System;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Interfaces
{
    public interface IPointCalculator
    {
        int CalculatePoints(bool userIsPro, int pointWeight, bool ownOrganizedEvent, DateTime deadline, UserRace result);

        int CalculatePoints(int billAmount);
    }
}
