using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Web;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Services;
using System.IO;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RaceResultsController : Controller
    {
        private readonly IRaceResultManager raceResultManager;
        private readonly CustomUserManager userManager; 
        private readonly IRaceManager raceManager;
        private readonly ILogger<RaceResultsController> logger;
        private readonly IStringLocalizer<SharedResources> localizer;

        public RaceResultsController(IRaceResultManager raceResultMgr, IRaceManager raceMgr, CustomUserManager userMgr, ILogger<RaceResultsController> raceResultsLogger, IStringLocalizer<SharedResources> loc)
        {
            userManager = userMgr;
            raceManager = raceMgr;
            logger = raceResultsLogger;
            localizer = loc;
            raceResultManager = raceResultMgr;
        }

        public async Task<IActionResult> Index(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            UserModel userModel = await userManager.GetUserByIdAsync(userId);
            if (userModel == null)
            {
                return NotFound();
            }

            User user = await userManager.FindByIdAsync(userModel.Id.ToString());
            UserResultsViewModel model = new UserResultsViewModel
            {
                Results = raceResultManager.ListRaceResultsByUserId(user.Id).OrderByDescending(x => x.RaceDate),
                FirstName = user.FirstName, 
                LastName = user.LastName, 
                UserId = user.Id
            };

            return View(model);
        }

        public IActionResult Add(int? id = null)
        {
           AddResultViewModel model = new AddResultViewModel();
           model.Races = raceManager.ListRacesForResultAdd();
           if (id.HasValue)
           {
               model.SelectedRaceId = id;
           }

           return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddResultViewModel model)
        {
            model.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model state for AddRaceResult.");
                model.Races = raceManager.ListRacesForResultAdd();
                return View(model);
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);

            MemoryStream imageStream = new MemoryStream();
            if(model.Image != null)
            {
                await model.Image.CopyToAsync(imageStream);
                imageStream.Position = 0;
            }

            await raceResultManager.AddResultAsync(user.Id, model.SelectedRaceId.Value, model.AbsoluteResult, model.CategoryResult, model.IsTakePartAsStaff, imageStream, model.Image.ContentType);

            return RedirectToAction("Index", "Points");
        }

        [Authorize(Roles = Roles.USER_MANAGER)]
        public async Task<IActionResult> AddResultToUser()
        {
            IList<RaceModel> races = raceManager.ListRacesForResultAdd();
            IEnumerable<UserModel> userModels = await userManager.ListUsersAsync();

            return View(new AddResultToUserViewModel(races, null, userModels, null));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.USER_MANAGER)]
        public async Task<IActionResult> AddResultToUser(AddResultToUserViewModel model)
        {
            model.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model state for AddRaceResultToUsers.");
                model.Races = raceManager.ListRacesForResultAdd();
                model.Users = await userManager.ListUsersAsync();
                return View(model);
            }

            MemoryStream imageStream = new MemoryStream();
            if (model.Image != null)
            {
                await model.Image.CopyToAsync(imageStream);
                imageStream.Position = 0;
            }

            await raceResultManager.AddResultAsync(model.SelectedUserId.Value, model.SelectedRaceId.Value, model.AbsoluteResult, model.CategoryResult, model.IsTakePartAsStaff, imageStream, model.Image.ContentType);
            
            return RedirectToAction(nameof(Index), new { id = model.SelectedUserId });
        }

        [Authorize(Roles = Roles.POINT_CONSUPTION_MANAGER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeImageValidationStatus(int resultId, int userId)
        {
            await raceResultManager.ChangeValidatedStatus(resultId);
            return RedirectToAction("Index", new { id = userId });
        }
    }
}