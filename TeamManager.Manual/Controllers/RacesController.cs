using System;
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
        private IRaceManager RaceManager { get; }
        private CustomUserManager UserManager { get; }

        public RacesController(IRaceManager raceMgr, CustomUserManager userMgr)
        {
            RaceManager = raceMgr;
            UserManager = userMgr;
        }
            
        public IActionResult Index() => View(RaceManager.ListRaces());

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

            await RaceManager.AddRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            RaceDetailsModel model = new RaceDetailsModel();
            RaceModel race = RaceManager.GetRaceById(id);
            if(race == null)
            {
                return NotFound();
            }

            model.BaseModel = race;
            model.EntriedRiders = await RaceManager.ListEntriedUsersAsync(id);

            User user = await UserManager.FindByNameAsync(User.Identity.Name);
            model.UserApplied = model.EntriedRiders.Contains(user);

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            RaceModel race = RaceManager.GetRaceById(id);
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

            await RaceManager.UpdateRaceAsync(race);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            await RaceManager.DeleteRaceAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}