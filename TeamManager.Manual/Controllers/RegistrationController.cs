using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    public class RegistrationController : Controller
    {
        private CustomUserManager UserManager { get; }

        public RegistrationController(CustomUserManager userManager)
        {
            UserManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(RegistrationViewModel model)
        {
            model.Validate(ModelState);
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

            IdentityResult createResult = await UserManager.CreateAsync(user, model.Password, address);
            if (!createResult.Succeeded)
            {
                AddModelError(createResult);
                return View();
            }

            return RedirectToAction(nameof(Index));
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