﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Web;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class PointsController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IRaceResultManager raceResultManager;
        private readonly IPointManager pointManager;
        private readonly ILogger<PointsController> logger;
        private readonly IBillManager billManager;
        private readonly IStringLocalizer<SharedResources> localizer;
        public PointsController(CustomUserManager customUserManager, IRaceResultManager raceResultMgr, IPointManager pointMgr, ILogger<PointsController> pointsControllerLogger, IBillManager bManager, IStringLocalizer<SharedResources> sharedResourcesLocalizer)
        {
            userManager = customUserManager;
            raceResultManager = raceResultMgr;
            pointManager = pointMgr;
            logger = pointsControllerLogger;
            billManager = bManager;
            localizer = sharedResourcesLocalizer;
        }

        public async Task<IActionResult> CollectedPoints(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(id);
            }

            string visitorUserId = userManager.GetUserId(User);
            if (!User.IsInRole(Roles.POINT_CONSUPTION_MANAGER) && user.Id != int.Parse(visitorUserId))
            {
                logger.LogWarning($"Unauthorized access to collected points of {user.FirstName} {user.LastName} by User.Id = {visitorUserId}");
                return Challenge();
            }

            IList<ResultModel> results = raceResultManager.ListRaceResultsByUserId(user.Id).ToList();
            CollectedPointsViewModel model = new CollectedPointsViewModel()
            {
                Results = results
            };

            return View(model);
        }

        public async Task<IActionResult> ConsumedPoints(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(id);
            }

            string visitorUserId = userManager.GetUserId(User);
            if (!User.IsInRole(Roles.POINT_CONSUPTION_MANAGER) && user.Id != int.Parse(visitorUserId))
            {
                logger.LogWarning($"Unauthorized access to consupted points of {user.FirstName} {user.LastName} by User.Id = {visitorUserId}");
                return Challenge();
            }

            IList<PointConsuption> consuptions = await pointManager.ListConsumedPointsAsync(int.Parse(userId));
            ConsumedPointsViewModel model = new ConsumedPointsViewModel()
            {
                UserId = user.Id,
                ConsumedPoints = consuptions
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

            model.Validate(ModelState, await pointManager.GetAvailablePointAmountByUser(int.Parse(model.SelectedUserId)), localizer);
            if(!ModelState.IsValid)
            {
                logger.LogDebug($"Invalid model for AddConsumedPoints.");
                model.Users = await GetUsersSelectList();
                return View(model);
            }

            string creatorUserId = userManager.GetUserId(User);
            await pointManager.AddConsumedPointAsync(int.Parse(model.SelectedUserId), model.Amount, creatorUserId, model.Remark);

            return RedirectToAction(nameof(ConsumedPoints), new { id = model.SelectedUserId });
        }

        private async Task<IEnumerable<SelectListItem>> GetUsersSelectList()
        {
            return (await userManager.ListUsersAsync()).Select(u => new SelectListItem(u.FullName, u.Id.ToString()));
        }
    }
}