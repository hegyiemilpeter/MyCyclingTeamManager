using System.Collections.Generic;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.ViewModels
{
    public class RaceDetailsViewModel
    {
        public RaceViewModel BaseModel { get; set; }

        public bool UserApplied { get; set; }

        public IList<string> EntriedRiders { get; set; }
    }
}
