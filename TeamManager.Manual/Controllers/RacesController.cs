using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Models;

namespace TeamManager.Manual.Controllers
{
    public class RacesController : Controller
    {
        private static IList<Race> Races
        {
            get
            {
                return new List<Race>()
                {
                    new Race() {
                        Name = "Bükk Kupa",
                        Date = new DateTime(2019,09,09),
                        Remark = "hegyi időfutam",
                        Distances = new List<RaceDistanceDetail>()
                        {
                            new RaceDistanceDetail() { RaceType = RaceType.Road_TimeTrial, Distance = 15 }
                        }
                    },
                    new Race() {
                        Name = "Tour de Velence",
                        Date = new DateTime(2019,09,14),
                        Distances = new List<RaceDistanceDetail>()
                        {
                            new RaceDistanceDetail() { RaceType = RaceType.Road_RoadRace, Distance = 130 },
                            new RaceDistanceDetail() { RaceType = RaceType.Road_RoadRace, Distance = 55 }
                        }
                    }
                };
            }
        }

        public IActionResult Index()
        {
            return View(Races);
        }
    }
}