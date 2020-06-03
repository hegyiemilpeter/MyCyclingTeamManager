using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Core.Services;
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

            IdentityResult createResult = await userManager.CreateAsync(model, model.Password, Url.Link("Default", new { controller = "Account", action = "Login" }));
            if (!createResult.Succeeded)
            {
                AddModelError(createResult);
                return View();
            }

            return View("RegistrationSuccess");
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