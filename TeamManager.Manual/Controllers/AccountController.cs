﻿using System;
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
        private readonly CustomUserManager userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(CustomUserManager customUserManager, SignInManager<User> signInMgr)
        {
            userManager = customUserManager;
            signInManager = signInMgr;
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

            User user = await userManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                await signInManager.SignOutAsync();
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, true);
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
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public async Task<IActionResult> Details(string userName)
        {
            UserModel  user = await userManager.GetUserByNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
}