using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamManager.Manual.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, bool> Roles { get; set; }
    }
}
