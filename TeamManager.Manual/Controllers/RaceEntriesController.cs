using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Data;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Services;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class RaceEntriesController : Controller
    {
        private readonly IRaceEntryManager raceEntryManager;
        private readonly CustomUserManager userManager;
        private readonly IRaceManager raceManager;

        public RaceEntriesController(IRaceEntryManager raceEntryMgr, IRaceManager raceMgr, CustomUserManager userMgr)
        {
            raceEntryManager = raceEntryMgr;
            userManager = userMgr;
            raceManager = raceMgr;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEntry(int id)
        {
            return await EditEntry(id, raceEntryManager.AddEntryAsync);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEntry(int id)
        {
            return await EditEntry(id, raceEntryManager.RemoveEntryAsync);
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