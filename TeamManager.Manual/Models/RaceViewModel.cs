using System;
using System.Collections.Generic;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class RaceViewModel : Race
    {
        public IList<int> Distances { get; set; }
    }
}
