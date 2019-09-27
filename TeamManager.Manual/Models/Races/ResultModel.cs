namespace TeamManager.Manual.Models
{
    public class ResultModel
    {
        public int UserId { get; set; }
        public int RaceId { get; set; }
        public string Race { get; set; }
        public int? AbsoluteResult { get; set; }
        public int? CategoryResult { get; set; }
        public bool IsStaff { get; set; }
        public bool IsDriver { get; set; }
        public int Points { get; set; }
        public int RacePointWeight { get; set; }
    }
}
