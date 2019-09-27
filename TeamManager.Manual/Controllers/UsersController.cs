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
        private readonly IPointManager pointManager;

        public UsersController(CustomUserManager customUserManager, IUserRaceManager userRaceMgr, IPointManager pointMgr)
        {
            userManager = customUserManager;
            userRaceManager = userRaceMgr;
            pointManager = pointMgr;
        }

        public async Task<IActionResult> Index()
        {
            return View(await userManager.ListUsersAsync());
        }

        public async Task<IActionResult> Details(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            UserModel userModel = await userManager.GetUserByIdAsync(userId);
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

        public async Task<IActionResult> Edit(int id)
        {
            User user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            UserModel model = await userManager.GetUserByNameAsync(user.UserName);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserModel model)
        {
            model.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await userManager.UpdateAsync(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}