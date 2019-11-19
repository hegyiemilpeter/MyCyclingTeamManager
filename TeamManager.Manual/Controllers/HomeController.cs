using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using TeamManager.Manual.Models.Interfaces;
using TeamManager.Manual.Models.ViewModels;

namespace TeamManager.Manual.Controllers
{
    public class HomeController : Controller
    {
        private IEmailSender emailSender;
        private IStringLocalizer<SharedResources> localizer;
        private IConfiguration configuration;
        public HomeController(IEmailSender sender, IStringLocalizer<SharedResources> loc, IConfiguration config)
        {
            emailSender = sender;
            localizer = loc;
            configuration = config;
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
                return View(model);
            }

            string emailTo = configuration.GetValue<string>("ContactEmail");
            if (string.IsNullOrEmpty(emailTo))
            {
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
        }
    }
}