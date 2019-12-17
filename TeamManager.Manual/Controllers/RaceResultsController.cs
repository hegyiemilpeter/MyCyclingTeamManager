using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RaceResultsController : Controller
    {
        private readonly IUserRaceManager userRaceManager; 
        private readonly CustomUserManager userManager; 
        private readonly IRaceManager raceManager;
        private readonly ILogger<RaceResultsController> logger;

        public RaceResultsController(IUserRaceManager userRaceMgr, IRaceManager raceMgr, CustomUserManager userMgr, ILogger<RaceResultsController> raceResultsLogger)
        {
            userRaceManager = userRaceMgr;
            userManager = userMgr;
            raceManager = raceMgr;
            logger = raceResultsLogger;
        }

        public IActionResult Add(int? id = null)
        {
           AddResultViewModel model = new AddResultViewModel();
           if(id != null)
           {
                model.SelectedRaceId = id;
           }

           model.Races = raceManager.ListPastRaces();
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
            model.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model state for AddRaceResult.");
                model.Races = raceManager.ListPastRaces();
                return View(model);
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            await userRaceManager.AddResultAsync(user, model.SelectedRaceId.Value, model.AbsoluteResult, model.CategoryResult, model.IsTakePartAsDriver, model.IsTakePartAsStaff, model.Image);

            return RedirectToAction("Index", "Points");
        }
    }
}