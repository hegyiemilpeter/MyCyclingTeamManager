using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    public class PointsController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IUserRaceManager userRaceManager;
        private readonly IPointManager pointManager;

        public PointsController(CustomUserManager customUserManager, IUserRaceManager userRaceMgr, IPointManager pointMgr)
        {
            userManager = customUserManager;
            userRaceManager = userRaceMgr;
            pointManager = pointMgr;
        }

        public async Task<IActionResult> Index(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            User user = await userManager.FindByIdAsync(userId);
            IList<ResultModel> results = userRaceManager.GetRaceResultsByUser(user);
            IList<PointConsuption> pointConsuptions = await pointManager.ListConsumedPointsAsync(userId);

            PointsViewModel model = new PointsViewModel()
            {
                UserId = user.Id,
                CollectedPoints = results,
                ConsumedPoints = pointConsuptions
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddConsumedPoints(string userId, int points, string remark)
        {
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            string creatorUserId = userManager.GetUserId(User);
            await pointManager.AddConsumedPointAsync(userId, points, creatorUserId, remark);

            return RedirectToAction(nameof(Index), new { id = userId });
        }
    }
}