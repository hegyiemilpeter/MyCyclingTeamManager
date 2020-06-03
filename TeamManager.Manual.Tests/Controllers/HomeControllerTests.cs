using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TeamManager.Manual.Controllers;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Web;

namespace TeamManager.Manual.Tests.Controllers
{
    public class HomeControllerTests
    {
        private Mock<IEmailSender> emailSender;
        private Mock<IStringLocalizer<SharedResources>> stringLocalizer;
        private Mock<IConfiguration> configuration;
        private Mock<ILogger<HomeController>> logger;

        [SetUp]
        public void Setup()
        {
            emailSender = new Mock<IEmailSender>();
            stringLocalizer = new Mock<IStringLocalizer<SharedResources>>();
            configuration = new Mock<IConfiguration>();
            logger = new Mock<ILogger<HomeController>>();
        }

        [Test]
        public void Test()
        {
            // Arrange
            HomeController homeController = new HomeController(emailSender.Object, stringLocalizer.Object, configuration.Object, logger.Object);

            // Act
            var response = homeController.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(response);
            Assert.IsNull((response as ViewResult).Model);
        }
    }
}
