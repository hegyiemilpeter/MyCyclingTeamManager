using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models
{
    public class Race
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Remark { get; set; }
        public IList<RaceDistanceDetail> Distances { get; set; }
    }
}
