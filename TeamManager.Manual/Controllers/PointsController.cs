using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Exceptions;
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
        private readonly ILogger<PointsController> logger;
        private readonly IBillManager billManager;

        public PointsController(CustomUserManager customUserManager, IUserRaceManager userRaceMgr, IPointManager pointMgr, ILogger<PointsController> pointsControllerLogger, IBillManager bManager)
        {
            userManager = customUserManager;
            userRaceManager = userRaceMgr;
            pointManager = pointMgr;
            logger = pointsControllerLogger;
            billManager = bManager;
        }

        public async Task<IActionResult> Index(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound(id);
            }

            string visitorUserId = userManager.GetUserId(User);
            if(!User.IsInRole(Roles.POINT_CONSUPTION_MANAGER) && user.Id != int.Parse(visitorUserId))
            {
                logger.LogWarning($"Unauthorized access to points of {user.FirstName} {user.LastName} by User.Id = {visitorUserId}");
                return Challenge();
            }

            IList<ResultModel> results = userRaceManager.GetRaceResultsByUser(user);
            IList<PointConsuption> pointConsuptions = await pointManager.ListConsumedPointsAsync(userId);
            BillModel bills = await billManager.ListBillsByUserAsync(user.Id);

            PointsViewModel model = new PointsViewModel()
            {
                UserId = user.Id,
                Results = results,
                ConsumedPoints = pointConsuptions,
                Bills = bills
            };

            return View(model);
        }

        [Authorize(Roles = Roles.POINT_CONSUPTION_MANAGER)]
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
        [Authorize(Roles = Roles.POINT_CONSUPTION_MANAGER)]
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
                logger.LogDebug($"Invalid model for AddConsumedPoints.");
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