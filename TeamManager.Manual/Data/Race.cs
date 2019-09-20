using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamManager.Manual.Data
{
    public class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public RaceType? TypeOfRace { get; set; }
        [Required]
        public DateTime? EntryDeadline { get; set; }
        [DataType(DataType.Url)]
        public string Website { get; set; }
        public int PointWeight { get; set; }
        public string Remark { get; set; }

        public virtual IEnumerable<RaceDistances> Distances { get; set; }
    }
}
