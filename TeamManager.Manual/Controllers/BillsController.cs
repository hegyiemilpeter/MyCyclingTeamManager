using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly ILogger<BillsController> logger;
        private readonly IBillManager billManager;
        private readonly CustomUserManager userManager;
        private readonly IStringLocalizer<SharedResources> localizer;

        public BillsController(ILogger<BillsController> billLogger, IBillManager billMgr, CustomUserManager userMgr, IStringLocalizer<SharedResources> stringLocalizer)
        {
            logger = billLogger;
            billManager = billMgr;
            userManager = userMgr;
            localizer = stringLocalizer;
        }

        public IActionResult Add() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BillViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model state in case of bill upload.");
                return View();
            }

            User user = await userManager.GetUserAsync(User);
            if(user == null)
            {
                return NotFound();
            }

            await billManager.CreateBillAsync(user, viewModel.Amount, viewModel.PurchaseDate.Value, viewModel.Image);
            ViewBag.Message = localizer["Successfully saved your bill."];
            return View();
        }
    }
}