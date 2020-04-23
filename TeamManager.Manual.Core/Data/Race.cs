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
        [Required(ErrorMessage = "The name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The date is required.")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "The city is required.")]
        public string City { get; set; }
        [Required]
        public RaceType? TypeOfRace { get; set; }
        public DateTime? EntryDeadline { get; set; }
        [DataType(DataType.Url)]
        public string Website { get; set; }
        public int PointWeight { get; set; }
        public string Remark { get; set; }
        public bool OwnOrganizedEvent { get; set; }

        public virtual IEnumerable<RaceDistance> Distances { get; set; }
    }
}
