﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;

namespace TeamManager.Manual.Controllers
{
    public class RacesController : Controller
    {
        private static IList<RaceViewModel> Races { get; set; } = 
            new List<RaceViewModel>()
                {
                    new RaceViewModel() {
                        Name = "Bükk Kupa",
                        Date = new DateTime(2019,09,09),
                        Remark = "hegyi időfutam",
                        City = "Miskolc",
                        EntryDeadline = new DateTime(2019,09,07),
                        Id = 1,
                        PointWeight = 1,
                        TypeOfRace = RaceType.Road_TimeTrial,
                        Distances = new List<int> { 15 }
                    },
                    new RaceViewModel() {
                        Name = "Tour de Velence",
                        Date = new DateTime(2019,09,14),
                        City = "Velence",
                        EntryDeadline = new DateTime(2019,09,07),
                        Id = 2,
                        PointWeight = 1,
                        TypeOfRace = RaceType.Road_RoadRace,
                        Website = "http://www.tosport.hu",
                        Distances = new List<int> { 55, 130 }
                    },
                    new RaceViewModel() {
                        Name = "Ljubljana Marathon",
                        Date = new DateTime(2019,10,14),
                        City = "Ljubljana",
                        Country = "Slovenia",
                        EntryDeadline = new DateTime(2019,10,07),
                        Id = 3,
                        PointWeight = 2,
                        TypeOfRace = RaceType.Road_RoadRace,
                        Distances = new List<int> { 60, 120, 150 }
                    }
                };
            
        public IActionResult Index()
        {
            return View(Races);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new RaceViewModel());
        }

        [HttpPost]
        public IActionResult Add(RaceViewModel race)
        {
            race.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                return View(race);
            }

            race.Id = Races.Count + 1;

            Races.Add(race);
            return RedirectToAction(nameof(Index));
        }
    }
}