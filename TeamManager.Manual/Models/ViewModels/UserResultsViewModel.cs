using System.Collections.Generic;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.ViewModels
{
    public class UserResultsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        public IEnumerable<ResultModel> Results { get; set; }
    }
}
