using System;
using TeamManager.Manual.Data;
using TeamManager.Manual.Core.Exceptions;
using TeamManager.Manual.Core.Interfaces;

namespace TeamManager.Manual.Core.Services
{
    public class PointCalculator : IPointCalculator
    {
        private static int STAFF_POINTS = 5;
        private static int ORGANIZER_POINTS = 5;
        private static int TOP_10_BONUS = 1;
        private static int TOP_3_BONUS = 2;
        private static int BILL_DIVIDER = 10000;
        private static int BASE_POINT = 3;

        public int CalculatePoints(int billAmount)
        {
            return billAmount / BILL_DIVIDER;
        }

        public int CalculatePoints(bool userIsPro, int raceWeight, bool ownOrganizedEvent, DateTime deadline, UserRace userRace)
        {
            if(userRace == null)
            {
                throw new PointCalculationException(userRace);
            }

            int result = 0;
            // Invalid result or Pro rider
            if ((userRace.ResultIsValid.HasValue && !userRace.ResultIsValid.Value) || userIsPro || DateTime.Now > deadline)
            {
                return result;
            }

            if (userRace.CategoryResult.HasValue && userRace.CategoryResult.Value > 0)
            {
                result += BASE_POINT * raceWeight;
                if (userRace.CategoryResult.Value <= 3)
                {
                    result += TOP_3_BONUS;
                }
                else if (userRace.CategoryResult.Value <= 10)
                {
                    result += TOP_10_BONUS;
                }
            }

            if (userRace.IsTakePartAsStaff.HasValue && userRace.IsTakePartAsStaff.Value)
            {
                if (ownOrganizedEvent)
                {
                    result += ORGANIZER_POINTS;
                }
                else
                {
                    result += STAFF_POINTS;
                }
            }

            return result;
        }
    }
}
