using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Models;

namespace TeamManager.Manual.Controllers
{
    public class RaceEntriesController : Controller
    {
        private static Dictionary<string, IList<string>> raceEntries = new Dictionary<string, IList<string>>();

        public RaceEntriesController()
        {
            raceEntries.Add("Tour de Zalakaros", new List<string> { "Tomi", "Peti", "Sama" });
            raceEntries.Add("Tour de Velence", new List<string> { "Emil" });
        }

        public IActionResult Index()
        {
            return View(raceEntries);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(RaceEntry entry)
        {
            if (raceEntries.ContainsKey(entry.RaceName))
            {
                raceEntries[entry.RaceName].Add(entry.RiderName);
            }
            else
            {
                raceEntries.Add(entry.RaceName, new List<string> { entry.RiderName });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}