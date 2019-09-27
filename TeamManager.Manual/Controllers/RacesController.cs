using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public RacesController(IRaceManager raceMgr, IUserRaceManager userRaceMgr, CustomUserManager userMgr)
        {
            raceManager = raceMgr;
            userRaceManager = userRaceMgr;
            userManager = userMgr;
        }
            
        public IActionResult Index() => View(raceManager.ListRaces());

        [HttpGet]
        public IActionResult Add() =>  View(new RaceModel());

        [HttpPost]
        public async Task<IActionResult> Add(RaceModel race)
        {
            race.Validate(ModelState);
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
        public async Task<IActionResult> Edit(RaceModel race)
        {
            race.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                return View(race);
            }

            await raceManager.UpdateRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await raceManager.DeleteRaceAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}