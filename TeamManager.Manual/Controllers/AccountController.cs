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
    public class AccountController : Controller
    {
        public CustomUserManager UserManager { get; }
        public SignInManager<User> SignInManager { get; }

        public AccountController(CustomUserManager customUserManager, SignInManager<User> signInMgr)
        {
            UserManager = customUserManager;
            SignInManager = signInMgr;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await UserManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                await SignInManager.SignOutAsync();
                var result = await SignInManager.PasswordSignInAsync(user, model.Password, false, true);
                if (result.Succeeded)
                {
                    return Redirect(returnUrl ?? "/");
                }
            }

            ModelState.AddModelError("", "Invalid e-mail or password.");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}