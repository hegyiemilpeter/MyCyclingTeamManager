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
        public IRaceManager RaceManager { get; set; }
        public CustomUserManager UserManager { get; set; }

        public RaceResultsController(IRaceManager raceManager, CustomUserManager userManager)
        {
            RaceManager = raceManager;
            UserManager = userManager;
        }

        public async Task<IActionResult> MyResults() 
        {
            User user = await UserManager.FindByNameAsync(User.Identity.Name);
            var model = await RaceManager.GetRaceResultsByUser(user);

            return View(model);
        }

        public IActionResult Add(int? id = null)
        {
           AddResultViewModel model = new AddResultViewModel();
           if(id != null)
           {
                model.SelectedRaceId = id;
           }

           model.Races = RaceManager.ListRaces().Where(x => x.Date.HasValue && x.Date.Value < DateTime.Now).ToList();
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
                model.Races = RaceManager.ListRaces();
                return View(model);
            }

            User user = await UserManager.FindByNameAsync(User.Identity.Name);
            await RaceManager.AddResultAsync(user, model.SelectedRaceId.Value, model.AbsoluteResult, model.CategoryResult, model.IsTakePartAsDriver, model.IsTakePartAsStaff);

            return RedirectToAction("MyResults");
        }
    }
}