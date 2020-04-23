using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Web;

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

        [Authorize(Roles = Roles.BILL_MANAGER)]
        public async Task<IActionResult> Overview()
        {
            IList<Bill> bills = await billManager.ListBillsAsync();
            return View(bills);
        }

        public async Task<IActionResult> Index(int? id = null)
        {
            string userId = id.HasValue ? id.Value.ToString() : userManager.GetUserId(User);
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(id);
            }

            string visitorUserId = userManager.GetUserId(User);
            if (!User.IsInRole(Roles.BILL_MANAGER) && user.Id != int.Parse(visitorUserId))
            {
                logger.LogWarning($"Unauthorized access to bills of {user.FirstName} {user.LastName} by User.Id = {visitorUserId}");
                return Challenge();
            }

            BillModel billsOfTheUser = await billManager.ListBillsByUserAsync(user.Id);
            IList<Bill> bills = billsOfTheUser != null && billsOfTheUser.Bills != null ? billsOfTheUser.Bills : new List<Bill>();
            return View("Overview", bills);
        }

        [HttpGet]
        [Authorize(Roles = Roles.BILL_MANAGER)]
        public async Task<IActionResult> Delete(int billId)
        {
            Bill bill = await billManager.GetBillByIdAsync(billId);
            if(bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.BILL_MANAGER)]
        public async Task<IActionResult> Delete(Bill bill)
        {
            await billManager.DeleteBillAsync(bill.Id);
            return RedirectToAction(nameof(Overview));
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
            return RedirectToAction("Index", "Points");
        }
    }
}