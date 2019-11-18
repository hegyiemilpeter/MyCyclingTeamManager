using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly IStringLocalizer<SharedResources> localizer;
         
        public RegistrationController(CustomUserManager userMgr, IStringLocalizer<SharedResources> localizer)
        {
            userManager = userMgr;
            this.localizer = localizer;
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
                TShirtSize = model.TShirtSize.Value
            };

            Dictionary<IdentificationNumberType, string> identifiers = CreateIdentifiersDictionaty(model);
            IdentityResult createResult = await userManager.CreateAsync(user, model.Password, address, identifiers);
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

        private Dictionary<IdentificationNumberType, string> CreateIdentifiersDictionaty(RegistrationViewModel model)
        {
            Dictionary<IdentificationNumberType, string> response = new Dictionary<IdentificationNumberType, string>();
            if (!string.IsNullOrEmpty(model.AKESZ)) { response.Add(IdentificationNumberType.AKESZ, model.AKESZ); }
            if (!string.IsNullOrEmpty(model.Otproba)) { response.Add(IdentificationNumberType.OtProba, model.Otproba); }
            if (!string.IsNullOrEmpty(model.UCI)) { response.Add(IdentificationNumberType.UCILicence, model.UCI); }
            if (!string.IsNullOrEmpty(model.Triathlon)) { response.Add(IdentificationNumberType.TriathleteLicence, model.Triathlon); }
            return response;
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