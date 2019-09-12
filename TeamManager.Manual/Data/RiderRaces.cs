using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class RiderRaces
    {
        [Key]
        public int Id { get; set; }
        public int RiderId { get; set; }
        public int RaceId { get; set; }
        public bool? IsEntryRequired { get; set; }
        public bool? IsEntryPaid { get; set; }
        public bool? IsPointsDisabled { get; set; }
        public bool? IsTakePartAsStaff { get; set; }
        public int? CategoryResult { get; set; }
        public int? AbsoluteResult { get; set; }
    }
}
