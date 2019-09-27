using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class RaceDetailsModel
    {
        public RaceModel BaseModel { get; set; }

        public bool UserApplied { get; set; }

        public IList<UserModel> EntriedRiders { get; set; }
    }
}
