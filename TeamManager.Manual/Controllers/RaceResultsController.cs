using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RaceResultsController : Controller
    {
        private readonly IUserRaceManager userRaceManager; 
        private readonly CustomUserManager userManager; 
        private readonly IRaceManager raceManager;

        public RaceResultsController(IUserRaceManager userRaceMgr, IRaceManager raceMgr, CustomUserManager userMgr)
        {
            userRaceManager = userRaceMgr;
            userManager = userMgr;
            raceManager = raceMgr;
        }

        public async Task<IActionResult> MyResults() 
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            IList<ResultModel> results = userRaceManager.GetRaceResultsByUser(user);

            return View(results);
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
        public async Task<IActionResult> Add(AddResultViewModel model)
        {
            model.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                model.Races = raceManager.ListPastRaces();
                return View(model);
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            await userRaceManager.AddResultAsync(user, model.SelectedRaceId.Value, model.AbsoluteResult, model.CategoryResult, model.IsTakePartAsDriver, model.IsTakePartAsStaff);

            return RedirectToAction("MyResults");
        }
    }
}