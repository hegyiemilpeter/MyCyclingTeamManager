using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Web;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Core;
using TeamManager.Manual.Core.Services;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly CustomUserManager userManager;
        private readonly CustomSignInManager signInManager;
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly ILogger<AccountController> logger;
        public AccountController(CustomUserManager customUserManager, CustomSignInManager signInMgr, IEmailSender sender, IStringLocalizer<SharedResources> stringLocalizer, ILogger<AccountController> aclogger)
        {
            userManager = customUserManager;
            signInManager = signInMgr;
            emailSender = sender;
            localizer = stringLocalizer;
            logger = aclogger;
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
                logger.LogDebug($"Invalid model in case of login. {model.Email}");
                return View(model);
            }

            User user = await userManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                await signInManager.SignOutAsync();

                if (!signInManager.UserIsVerified(user))
                {
                    ModelState.AddModelError("", localizer["User is not validated by admins."]);
                    ViewBag.ReturnUrl = returnUrl;
                    return View();
                }

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
                logger.LogDebug($"Invalid model in case of forgot password. {email}");
                return View();
            }

            User currentUser = await userManager.FindByEmailAsync(email);
            if(currentUser != null)
            {
                await userManager.SendForgotPasswordEmailAsync(currentUser, HttpContext.Request.Scheme + "://" + HttpContext.Request.Host);    
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
                logger.LogDebug($"Invalid model in case of reset password. {model.ToString()}");
                return View(model);
            }

            User currentUser = await userManager.FindByIdAsync(model.UserId);
            if(currentUser == null)
            {
                ModelState.AddModelError(nameof(model.UserId), localizer["No user can be found with the given id."]);
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