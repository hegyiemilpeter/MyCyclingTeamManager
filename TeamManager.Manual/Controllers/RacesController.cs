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

        public RacesController(IRaceManager raceMgr)
        {
            raceManager = raceMgr;
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
    }
}