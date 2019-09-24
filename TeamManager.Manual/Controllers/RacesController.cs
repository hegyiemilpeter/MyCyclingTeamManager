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
            RaceModel race = RaceManager.GetById(id);
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
        public async Task<IActionResult> Edit(int id)
        {
            RaceDetailsModel model = new RaceDetailsModel();
            RaceModel race = RaceManager.GetById(id);
            if (race == null)
            {
                return NotFound();
            }

            model.BaseModel = race;
            model.EntriedRiders = await RaceManager.ListEntriedUsersAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry(int id)
        {
            return await EditEntry(id, RaceManager.AddEntryAsync);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveEntry(int id)
        {
            return await EditEntry(id, RaceManager.RemoveEntryAsync);
        }

        private async Task<IActionResult> EditEntry(int id, Func<User,Race, Task> functionToPerform)
        {
            RaceModel race = RaceManager.GetById(id);
            if (race == null)
            {
                return NotFound();
            }

            User user = await UserManager.FindByNameAsync(User.Identity.Name);
            await functionToPerform.Invoke(user, race);

            return RedirectToAction(nameof(Details), new { id = race.Id });
        }
    }
}