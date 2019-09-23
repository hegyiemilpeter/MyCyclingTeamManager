using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RacesController : Controller
    {
        private IRaceManager raceManager { get; }
        private CustomUserManager userManager { get; }

        public RacesController(IRaceManager raceMgr, CustomUserManager userMgr)
        {
            raceManager = raceMgr;
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
            RaceModel race = raceManager.GetById(id);
            if(race == null)
            {
                return NotFound();
            }

            model.BaseModel = race;
            model.EntriedRiders = await raceManager.ListEntriedUsersAsync(id);

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            model.UserApplied = model.EntriedRiders.Contains(user);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Entry(int id)
        {
            RaceModel race = raceManager.GetById(id);
            if (race == null)
            {
                return NotFound();
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            await raceManager.AddEntryAsync(user, race);

            ViewBag.Message = $"Successful entry for {race.Name}";
            return RedirectToAction(nameof(Details), new { id = race.Id });
        }
    }
}