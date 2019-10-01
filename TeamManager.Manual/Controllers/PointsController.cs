using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> AddConsumedPoints(int? userId = null)
        {
            AddPointConsumptionViewModel model = new AddPointConsumptionViewModel();
            model.Users = await GetUsersSelectList();
            if (userId.HasValue)
            {
                model.SelectedUserId = userId.Value.ToString();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConsumedPoints(AddPointConsumptionViewModel model)
        {
            User user = await userManager.FindByIdAsync(model.SelectedUserId);
            if (user == null)
            {
                return NotFound();
            }

            model.Validate(ModelState, await pointManager.GetAvailablePointAmountByUser(model.SelectedUserId));
            if(!ModelState.IsValid)
            {
                model.Users = await GetUsersSelectList();
                return View(model);
            }

            string creatorUserId = userManager.GetUserId(User);
            await pointManager.AddConsumedPointAsync(model.SelectedUserId, model.Amount, creatorUserId, model.Remark);

            return RedirectToAction(nameof(Index), new { id = model.SelectedUserId });
        }

        private async Task<IEnumerable<SelectListItem>> GetUsersSelectList()
        {
            return (await userManager.ListUsersAsync()).Select(u => new SelectListItem(u.FullName, u.Id.ToString()));
        }
    }
}