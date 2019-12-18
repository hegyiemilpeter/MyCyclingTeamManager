using System.Collections.Generic;

namespace TeamManager.Manual.Models.ViewModels
{
    public class UserResultsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        public IEnumerable<ResultModel> Results { get; set; }
    }
}
