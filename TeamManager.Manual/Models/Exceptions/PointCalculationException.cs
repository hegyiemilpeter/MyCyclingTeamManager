using System;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.Exceptions
{
    public class PointCalculationException : Exception
    {
        public PointCalculationException(UserRace result) : base(result == null ? "Result object is null" : (result.User == null ? "User object is null" : (result.Race == null ? "Result object is null" : string.Empty)))
        {
        }

        public PointCalculationException(string message) : base(message)
        {
        }
    }
}
