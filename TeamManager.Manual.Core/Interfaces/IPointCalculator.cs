using System;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Interfaces
{
    public interface IPointCalculator
    {
        int CalculatePoints(bool userIsPro, int pointWeight, bool ownOrganizedEvent, DateTime deadline, UserRace result);

        int CalculatePoints(int billAmount);
    }
}
