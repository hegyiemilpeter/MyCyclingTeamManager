using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Web;

namespace TeamManager.Manual.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly ILogger<RegistrationController> logger;
         
        public RegistrationController(CustomUserManager userMgr, IStringLocalizer<SharedResources> registrationLocalizer, ILogger<RegistrationController> registrationLogger)
        {
            userManager = userMgr;
            localizer = registrationLocalizer;
            logger = registrationLogger;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegistrationViewModel model)
        {
            model.Validate(ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug("Invalid model state for registration.");
                return View();
            }

            Address address = new Address()
            {
                City = model.City,
                Country = model.Country,
                HouseNumber = model.HouseNumber,
                Street = model.Street,
                ZipCode = model.ZipCode
            };

            User user = new User()
            {
                BirthDate = model.BirthDate,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender.Value,
                PhoneNumber = model.PhoneNumber,
                TShirtSize = model.TShirtSize.Value,
                AkeszNumber = model.AKESZ,
                OtprobaNumber = model.Otproba,
                TriathleteLicence = model.Triathlon,
                UCILicence = model.UCI,
                BirthPlace = model.BirthPlace,
                IDNumber = model.IDNumber,
                MothersName = model.MothersName
            };

            IdentityResult createResult = await userManager.CreateAsync(user, model.Password, address, Url.Link("Default", new { controller = "Account", action = "Login" }));
            if (!createResult.Succeeded)
            {
                AddModelError(createResult);
                return View();
            }

            return RedirectToAction(nameof(RegistrationSuccess));
        }

        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        private void AddModelError(IdentityResult createResult)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}