using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IUserRaceManager userRaceManager;

        public UsersController(CustomUserManager customUserManager, IUserRaceManager userRaceMgr)
        {
            userManager = customUserManager;
            userRaceManager = userRaceMgr;
        }

        public async Task<IActionResult> Index()
        {
            return View(await userManager.ListUsersAsync());
        }

        public async Task<IActionResult> Details(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = User.Identity.Name;
            }

            UserModel userModel = await userManager.GetUserByNameAsync(userName);
            if (userModel == null)
            {
                return NotFound();
            }

            UserDetailsViewModel model = new UserDetailsViewModel()
            {
                UserData = userModel
            };

            User user = await userManager.FindByIdAsync(userModel.Id.ToString());
            model.Results = userRaceManager.GetRaceResultsByUser(user);

            return View(model);
        }
    }
}