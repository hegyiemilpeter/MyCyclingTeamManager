using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.ViewModels;
using TeamManager.Manual.Web;

namespace TeamManager.Manual.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailSender emailSender;
        private readonly IStringLocalizer<SharedResources> localizer;
        private readonly IConfiguration configuration;
        private readonly ILogger<HomeController> logger;
        public HomeController(IEmailSender sender, IStringLocalizer<SharedResources> loc, IConfiguration config, ILogger<HomeController> homeLogger)
        {
            emailSender = sender;
            localizer = loc;
            configuration = config;
            logger = homeLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            SetNewValidationNumber();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            byte[] validationBytes;
            HttpContext.Session.TryGetValue("ContactValidation", out validationBytes);
            SetNewValidationNumber();

            model.Validate(validationBytes, ModelState, localizer);
            if (!ModelState.IsValid)
            {
                logger.LogDebug($"Invalid model for contact. {model.ToString()}");
                return View(model);
            }

            string emailTo = configuration.GetValue<string>("ContactEmail");
            if (string.IsNullOrEmpty(emailTo))
            {
                logger.LogError($"Contact email is not configured.");
                throw new ApplicationException("Contact email is not configured.");
            }

            await emailSender.SendContactEmailAsync(emailTo, model.Message, model.Email);
            ViewBag.Message = localizer["Successfully sent contact e-mail!"];
            ModelState.Clear();
            return View();
        }

        private void SetNewValidationNumber()
        {
            Random random = new Random();
            byte sessionVarialbe = (byte)random.Next(0, byte.MaxValue);
            HttpContext.Session.Set("ContactValidation", new byte[] { sessionVarialbe });
            ViewBag.SessionVariable = sessionVarialbe;
            logger.LogDebug("Validation number generated for contacting.");
        }
    }
}