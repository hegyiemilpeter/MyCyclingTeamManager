using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class PointCalculator : IPointCalculator
    {
        private static int STAFF_POINTS = 1;
        private static int ORGANIZER_POINTS = 5;
        private static int DRIVER_POINTS = 1;
        private static int TOP_10_BONUS = 1;
        private static int TOP_3_BONUS = 2;

        public int CalculatePoints(int raceWeight, bool ownEvent, int? categoryResult, bool? staff, bool? driver)
        {
            int result = 0;
            if (categoryResult.HasValue && categoryResult.Value > 0)
            {
                if (categoryResult.Value <= 3)
                {
                    result += TOP_3_BONUS;
                }
                else if (categoryResult.Value <= 10)
                {
                    result += TOP_10_BONUS;
                }

                result += raceWeight;
            }

            if (staff.HasValue && staff.Value)
            {
                if (ownEvent)
                {
                    result += ORGANIZER_POINTS;
                }
                else
                {
                    result += STAFF_POINTS;
                }
            }

            if (driver.HasValue && driver.Value)
            {
                result += DRIVER_POINTS;
            }

            return result;
        }
    }
}
