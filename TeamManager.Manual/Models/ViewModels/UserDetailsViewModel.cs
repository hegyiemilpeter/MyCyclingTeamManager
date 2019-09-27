using System.Collections.Generic;

namespace TeamManager.Manual.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public UserModel UserData { get; set; }
        public IEnumerable<ResultModel> Results { get; set; }
    }
}
