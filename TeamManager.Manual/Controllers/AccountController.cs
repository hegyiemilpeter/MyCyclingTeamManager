﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly CustomSignInManager signInManager;
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<SharedResources> localizer;

        public AccountController(CustomUserManager customUserManager, CustomSignInManager signInMgr, IEmailSender sender, IStringLocalizer<SharedResources> stringLocalizer)
        {
            userManager = customUserManager;
            signInManager = signInMgr;
            emailSender = sender;
            localizer = stringLocalizer;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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

            ModelState.AddModelError("", localizer["Invalid e-mail or password."]);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required(ErrorMessage = "The Email address is required.")]string email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User currentUser = await userManager.FindByEmailAsync(email);
            if(currentUser != null)
            {
                string passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(currentUser);
                string encodedToken = HttpUtility.UrlEncode(passwordResetToken);
                await emailSender.SendForgotPasswordEmailAsync(currentUser.Email, currentUser.FirstName, encodedToken, currentUser.Id.ToString(), HttpContext.Request.Scheme + "://" + HttpContext.Request.Host);
            }

            return RedirectToAction(nameof(SuccessfulTokenGeneration));
        }
        
        [AllowAnonymous]
        public IActionResult SuccessfulTokenGeneration() => View();

        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string userId)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel()
            {
                Token = token,
                UserId = userId
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User currentUser = await userManager.FindByIdAsync(model.UserId);
            if(currentUser == null)
            {
                ModelState.AddModelError(nameof(model.UserId), "No user can be found with the given id.");
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                return View(model);
            }

            IdentityResult changePasswordResult = await userManager.ResetPasswordAsync(currentUser, model.Token, model.Password);
            if (!changePasswordResult.Succeeded)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;

                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Login));
        }
    }
}