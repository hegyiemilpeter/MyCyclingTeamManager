using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models
{
    public static class Roles
    {
        public const string POINT_CONSUPTION_MANAGER = "Point Consuption Manager";
        public const string RACE_MANAGER = "Race Manager";
        public const string USER_MANAGER = "User Manager";

        public static IEnumerable<string> GetAllRoles()
        {
            return new[] { POINT_CONSUPTION_MANAGER, RACE_MANAGER, USER_MANAGER };
        }
    }
}
