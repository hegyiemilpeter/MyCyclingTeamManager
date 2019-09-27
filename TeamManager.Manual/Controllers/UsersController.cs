using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamManager.Manual.Models;

namespace TeamManager.Manual.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly CustomUserManager userManager;

        public UsersController(CustomUserManager customUserManager)
        {
            userManager = customUserManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await userManager.ListUsersAsync());
        }
    }
}