namespace TeamManager.Manual.Models.ViewModels
{
    public class ResultViewModel
    {
        public string Race { get; set; }
        public int? AbsoluteResult { get; set; }
        public int? CategoryResult { get; set; }
        public bool IsStaff { get; set; }
        public bool IsDriver { get; set; }
        public int Points { get; set; }
    }
}
