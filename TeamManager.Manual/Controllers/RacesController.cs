using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RacesController : Controller
    {
        private readonly IRaceManager raceManager;
        private readonly CustomUserManager userManager;
        private readonly IUserRaceManager userRaceManager;
        private readonly IStringLocalizer<SharedResources> localizer;

        public RacesController(IRaceManager raceMgr, IUserRaceManager userRaceMgr, CustomUserManager userMgr, IStringLocalizer<SharedResources> localizer)
        {
            raceManager = raceMgr;
            userRaceManager = userRaceMgr;
            userManager = userMgr;
            this.localizer = localizer;
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

            IList<RaceModel> model = raceManager.ListRaces()
                .Where(x => x.Date.HasValue && x.Date.Value.Year == year && x.Date.Value.Month == month)
                .OrderBy(r => r.Date).ToList();

            ViewBag.Year = year;
            ViewBag.Month = month;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        public IActionResult Add() =>  View(new RaceModel());

        [HttpPost]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RaceModel race)
        {
            race.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                return View(race);
            }

            await raceManager.AddRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            RaceDetailsModel model = new RaceDetailsModel();
            RaceModel race = raceManager.GetRaceById(id);
            if(race == null)
            {
                return NotFound();
            }

            model.BaseModel = race;
            model.EntriedRiders = await userRaceManager.ListEntriedUsersAsync(id);

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            model.UserApplied = model.EntriedRiders.Any(x => x.Id == user.Id);

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

            return View(race);
        }

        [HttpPost]
        [Authorize(Roles = Roles.RACE_MANAGER)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RaceModel race)
        {
            race.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
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