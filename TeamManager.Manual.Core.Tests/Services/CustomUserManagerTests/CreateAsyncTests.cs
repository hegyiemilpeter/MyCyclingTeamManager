using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Repository;
using TeamManager.Manual.Core.Services;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Tests
{
    public class Tests
    {
        Mock<CustomUserManager> userManager;
        Mock<UnitOfWork> unitOfWork;

        [SetUp]
        public void Setup()
        {
            unitOfWork = new Mock<UnitOfWork>((TeamManagerDbContext)null);
            userManager = new Mock<CustomUserManager>(MockBehavior.Loose, unitOfWork.Object, new Mock<IConfiguration>().Object, new Mock<IEmailSender>().Object, new Mock<IUserStore<User>>().Object, new Mock<IOptions<IdentityOptions>>().Object, new Mock<IPasswordHasher<User>>().Object, null, null, null, null, null, null);
        }

        [Test]
        public async Task CreateAsync_Success_VerifiedUser()
        {
            UserModel model = CreateUserModel();

            userManager.Setup(x => x.CreateAsync(It.Is<User>(x => x.Email == model.Email), "pw")).Returns(Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.IsEmailOnWildCardList(model.Email)).Returns(true);
            userManager.Setup(x => x.VerifyUserAsync(It.Is<User>(x => x.Email == model.Email), "url")).Returns(Task.CompletedTask);

            IdentityResult result = await userManager.Object.CreateAsync(model, "pw", "url");
            Assert.IsTrue(result.Succeeded);
            userManager.Verify(x => x.CreateAsync(It.Is<User>(x => x.Email == model.Email), "pw"));
            userManager.Verify(x => x.VerifyUserAsync(It.Is<User>(x => x.Email == model.Email), "url"));
        }

        [Test]
        public async Task CreateAsync_Success_NotVerifiedUser()
        {
            UserModel model = CreateUserModel();

            userManager.Setup(x => x.CreateAsync(It.Is<User>(x => x.Email == model.Email), "pw")).Returns(Task.FromResult(IdentityResult.Success));
            userManager.Setup(x => x.IsEmailOnWildCardList(model.Email)).Returns(false);

            IdentityResult result = await userManager.Object.CreateAsync(model, "pw", "url");
            Assert.IsTrue(result.Succeeded);
            userManager.Verify(x => x.CreateAsync(It.Is<User>(x => x.Email == model.Email), "pw"));
        }

        private static UserModel CreateUserModel()
        {
            return new UserModel()
            {
                AKESZ = "1234",
                BirthDate = new DateTime(1989, 02, 02),
                BirthPlace = "Bukarest",
                City = "Test",
                Email = "testing@gmail.com",
                FirstName = "Teszt",
                Gender = Gender.Female,
                HouseNumber = "25",
                Id = 34,
                IDNumber = "5754",
                IsPro = false,
                LastName = "Elek",
                MothersName = "Anyuka",
                Otproba = string.Empty,
                PhoneNumber = "+36-12-134-3566",
                Street = "Teszsst",
                TShirtSize = Size.S,
                UCI = "ROM12345",
                ZipCode = "34556"
            };
        }
    }
}