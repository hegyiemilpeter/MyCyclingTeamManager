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
        private IRaceManager RaceManager { get; }
        private CustomUserManager UserManager { get; }

        public RaceEntriesController(IRaceManager raceMgr, CustomUserManager userMgr)
        {
            RaceManager = raceMgr;
            UserManager = userMgr;
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

        private async Task<IActionResult> EditEntry(int id, Func<User, Race, Task> functionToPerform)
        {
            RaceModel race = RaceManager.GetRaceById(id);
            if (race == null)
            {
                return NotFound();
            }

            User user = await UserManager.FindByNameAsync(User.Identity.Name);
            await functionToPerform.Invoke(user, race);

            return RedirectToAction("Details", "Races", new { id = race.Id });
        }
    }
}