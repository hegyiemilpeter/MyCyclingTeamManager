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
    public class RaceEntriesController : Controller
    {
        private readonly IUserRaceManager userRaceManager;
        private readonly CustomUserManager userManager;
        private readonly IRaceManager raceManager;

        public RaceEntriesController(IUserRaceManager userRaceMgr, IRaceManager raceMgr, CustomUserManager userMgr)
        {
            userRaceManager = userRaceMgr;
            userManager = userMgr;
            raceManager = raceMgr;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEntry(int id)
        {
            return await EditEntry(id, userRaceManager.AddEntryAsync);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEntry(int id)
        {
            return await EditEntry(id, userRaceManager.RemoveEntryAsync);
        }

        private async Task<IActionResult> EditEntry(int id, Func<User, RaceModel, Task> functionToPerform)
        {
            RaceModel race = raceManager.GetRaceById(id);
            if (race == null)
            {
                return NotFound();
            }

            User user = await userManager.FindByNameAsync(User.Identity.Name);
            await functionToPerform.Invoke(user, race);

            return RedirectToAction("Details", "Races", new { id = race.Id });
        }
    }
}