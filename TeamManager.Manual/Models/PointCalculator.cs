﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
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

            result += BASE_POINT;
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
