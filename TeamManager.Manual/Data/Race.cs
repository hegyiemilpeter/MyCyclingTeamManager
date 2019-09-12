using System;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class Race
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public RaceType TypeOfRace { get; set; }
        public DateTime EntryDeadline { get; set; }
        [DataType(DataType.Url)]
        public string Website { get; set; }
        public int PointWeight { get; set; }
        public string Remark { get; set; }
    }
}
