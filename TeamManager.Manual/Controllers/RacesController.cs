using System;
using System.Collections;
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
using TeamManager.Manual.Web;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Core.Services;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RacesController : Controller
    {
        private readonly IRaceManager raceManager;
        private readonly CustomUserManager userManager;
        private readonly IRaceEntryManager raceEntryManager;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly ILogger<RacesController> logger;

        public RacesController(IRaceManager raceMgr, IRaceEntryManager raceEntryMgr, CustomUserManager userMgr, IStringLocalizer<SharedResources> localizer, ILogger<RacesController> racesLogger)
        {
            raceManager = raceMgr;
            raceEntryManager = raceEntryMgr;
            userManager = userMgr;
            this.localizer = localizer;
            logger = racesLogger;
        }

        public IActionResult Index(int? year, int? month)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            if (!month.HasValue || month.Value < 1 || month.Value > 12)
            {
                month = DateTime.Now.Month;
            }

            IList<RaceViewModel> model = raceManager.ListRaces()
                .Where(x => x.Date.HasValue && x.Date.Value.Year == year && x.Date.Value.Month == month)
                .OrderBy(r => r.Date)
                .Select(x => new RaceViewModel(x))
                .ToList();

            ViewBag.Year = year;
            ViewBag.Month = month;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        public IActionResult Add() =>  View(new RaceViewModel() { PointWeight = 1 });

        [HttpPost]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RaceViewModel race)
        {
            race.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model for add race.");
                return View(race);
            }

            await raceManager.AddRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            RaceDetailsViewModel model = new RaceDetailsViewModel();
            RaceModel race = raceManager.GetRaceById(id);
            if(race == null)
            {
                return NotFound();
            }

            model.BaseModel = new RaceViewModel(raceManager.GetRaceById(id));
            model.EntriedRiders = await raceEntryManager.ListEntriedUsersAsync(id);

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            model.UserApplied = model.EntriedRiders.Contains($"{user.FirstName} {user.LastName}");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        public IActionResult Edit(int id)
        {
            RaceModel race = raceManager.GetRaceById(id);
            if (race == null)
            {
                return NotFound();
            }

            RaceViewModel viewModel = new RaceViewModel(race);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RaceViewModel race)
        {
            race.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model for edit race.");
                return View(race);
            }

            await raceManager.UpdateRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Roles.RACE_MANAGER)]
        public async Task<IActionResult> Delete(int id)
        {
            await raceManager.DeleteRaceAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}