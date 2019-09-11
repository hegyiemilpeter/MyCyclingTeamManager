using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models
{
    public class Race
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Remark { get; set; }
        public IList<RaceDistanceDetail> Distances { get; set; }
    }
}
