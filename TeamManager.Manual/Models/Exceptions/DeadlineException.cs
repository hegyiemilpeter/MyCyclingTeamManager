using System;

namespace TeamManager.Manual.Models.Exceptions
{
    public class DeadlineException : Exception
    {
        public DeadlineException() : base("The deadline is over, modifications are not allowed")
        {

        }
    }
}
