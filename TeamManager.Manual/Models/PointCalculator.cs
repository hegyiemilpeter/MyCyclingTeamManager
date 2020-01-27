﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class PointCalculator : IPointCalculator
    {
        private static int STAFF_POINTS = 2;
        private static int ORGANIZER_POINTS = 2;
        private static int TOP_10_BONUS = 1;
        private static int TOP_3_BONUS = 2;
        private static int BILL_DIVIDER = 10000;

        public int CalculatePoints(int billAmount)
        {
            return billAmount / BILL_DIVIDER;
        }

        public int CalculatePoints(int raceWeight, bool ownEvent, UserRace userRace)
        {
            int result = 0;
            if (userRace.ResultIsValid.HasValue && !userRace.ResultIsValid.Value)
            {
                return result;
            }

            result += 3;
            if (userRace.CategoryResult.HasValue && userRace.CategoryResult.Value > 0)
            {
                if (userRace.CategoryResult.Value <= 3)
                {
                    result += TOP_3_BONUS;
                }
                else if (userRace.CategoryResult.Value <= 10)
                {
                    result += TOP_10_BONUS;
                }

                result += raceWeight;
            }

            if (userRace.IsTakePartAsStaff.HasValue && userRace.IsTakePartAsStaff.Value)
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

            return result;
        }
    }
}
