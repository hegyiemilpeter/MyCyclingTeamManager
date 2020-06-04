using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Web;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.ViewModels;
using System;
using TeamManager.Manual.Core.Services;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IUserRaceManager userRaceManager;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly ILogger<UsersController> logger;

        public UsersController(CustomUserManager customUserManager, IUserRaceManager userRaceMgr, IStringLocalizer<SharedResources> userLocalizer, ILogger<UsersController> usersLogger)
        {
            userManager = customUserManager;
            userRaceManager = userRaceMgr;
            localizer = userLocalizer;
            logger = usersLogger;
        }

        public async Task<IActionResult> Index()
        {
            return View((await userManager.ListUsersAsync()).Select(x => new UserViewModel(x)).ToList());
        }

        public async Task<IActionResult> Details(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            string visitorUserId = userManager.GetUserId(User);
            if(!User.IsInRole(Roles.USER_MANAGER) && userId != visitorUserId)
            {
                logger.LogWarning($"{visitorUserId} is not authorized to visit User {userId}'s user details.");
                return Challenge();
            }

            UserModel userModel = await userManager.GetUserByIdAsync(userId);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(new UserViewModel(userModel));
        }

        public async Task<IActionResult> Edit(int id)
        {
            User user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            string visitorUserId = userManager.GetUserId(User);
            if (!User.IsInRole(Roles.USER_MANAGER) && id.ToString() != visitorUserId)
            {
                logger.LogWarning($"{visitorUserId} is not authorized to edit User {user.Email}'s user data.");
                return Challenge();
            }

            UserModel model = await userManager.GetUserByNameAsync(user.UserName);
            UserViewModel viewModel = new UserViewModel(model);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            model.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model for edit user.");
                return View(model);
            }

            string visitorUserId = userManager.GetUserId(User);
            if (model.Id != int.Parse(visitorUserId) && !User.IsInRole(Roles.USER_MANAGER))
            {
                logger.LogWarning($"{visitorUserId} is not authorized to edit User {model.Email}'s user data.");
                return Challenge();
            }

            await userManager.UpdateAsync(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = Roles.USER_MANAGER)]
        public async Task<IActionResult> Verify(int id)
        {
            User userModel = await userManager.FindByIdAsync(id.ToString());
            if (userModel == null)
            {
                return NotFound();
            }

            await userManager.VerifyUserAsync(userModel, Url.Link("Default", new { controller = "Account", action = "Login" }));
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}