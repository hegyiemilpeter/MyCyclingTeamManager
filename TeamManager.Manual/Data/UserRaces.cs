using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class UserRace
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RaceId { get; set; }
        public bool? IsEntryRequired { get; set; }
        public bool? IsTakePartAsStaff { get; set; }
        public bool? IsTakePartAsDriver { get; set; }

        public int? CategoryResult { get; set; }
        public int? AbsoluteResult { get; set; }
        public string ImageUrl { get; set; }
        public bool? ResultIsValid { get; set; }

        public virtual Race Race { get; set; }
        public virtual User User { get; set; }
    }
}
