using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models;
using TeamManager.Manual.ViewModels;

namespace TeamManager.Manual.Controllers
{
    [Authorize(Policy = "Developers")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly CustomUserManager userManager;

        public RolesController(RoleManager<IdentityRole<int>> roleMgr, CustomUserManager customUserManager)
        {
            roleManager = roleMgr;
            userManager = customUserManager;
        }

        public async Task<IActionResult> Index()
        {
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();

            await CreateDefaultRolesAndAddCurrentDeveloper();

            List<User> users = userManager.Users.ToList();
            List<IdentityRole<int>> roles = roleManager.Roles.ToList();
            foreach (var user in users)
            {
                UserRolesViewModel viewModel = new UserRolesViewModel();
                viewModel.Name = user.FirstName + " " + user.LastName;
                viewModel.UserId = user.Id;
                viewModel.Roles = new Dictionary<string, bool>();
                foreach (var role in roles)
                {
                    bool isInRole = await userManager.IsInRoleAsync(user, role.Name);
                    viewModel.Roles.Add(role.Name, isInRole);
                }

                model.Add(viewModel);
            }

            return View(model);
        }
        
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Required] string roleName)
        {
            IdentityRole<int> existingRole = await roleManager.FindByNameAsync(roleName);
            if(existingRole == null)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole<int>()
                {
                    Name = roleName
                });

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        return View();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserToRole(string userId, string roleName)
        {
            User user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return NotFound(userId);
            }

            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return NotFound(roleName);
            }

            await userManager.AddToRoleAsync(user, roleName);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromRole(string userId, string roleName)
        {
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(userId);
            }

            bool roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return NotFound(roleName);
            }

            await userManager.RemoveFromRoleAsync(user, roleName);
            return RedirectToAction(nameof(Index));
        }

        private async Task CreateDefaultRolesAndAddCurrentDeveloper()
        {
            foreach (var role in Roles.GetAllRoles())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    IdentityRole<int> identityRole = new IdentityRole<int>(role);
                    await roleManager.CreateAsync(identityRole);
                }

                if (!User.IsInRole(role))
                {
                    User user = await userManager.GetUserAsync(User);
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

    }
}