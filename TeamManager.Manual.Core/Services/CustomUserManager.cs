﻿using Diacritics.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using TeamManager.Manual.Core.Exceptions;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Core.Repository;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Core.Services
{
    public class CustomUserManager : UserManager<User>
    {
        private UnitOfWork UnitOfWork { get; }
        private IConfiguration Configuration { get; }
        private IEmailSender EmailSender { get; }

        public CustomUserManager(TeamManagerDbContext dbContext, IConfiguration configuration, IEmailSender emailSender, IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            UnitOfWork = new UnitOfWork(dbContext);
            Configuration = configuration;
            EmailSender = emailSender;
        }

        // Only for unit testing purposes
        protected internal CustomUserManager(UnitOfWork unitOfWork, IConfiguration configuration, IEmailSender emailSender, IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            UnitOfWork = unitOfWork;
            Configuration = configuration;
            EmailSender = emailSender;
        }

        public async Task<IdentityResult> CreateAsync(UserModel model, string password, string loginUrl)
        {
            Address address = new Address()
            {
                City = model.City,
                Country = model.Country,
                HouseNumber = model.HouseNumber,
                Street = model.Street,
                ZipCode = model.ZipCode
            };

            User user = new User()
            {
                BirthDate = model.BirthDate,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender.Value,
                PhoneNumber = model.PhoneNumber,
                TShirtSize = model.TShirtSize.Value,
                AkeszNumber = model.AKESZ,
                OtprobaNumber = model.Otproba,
                TriathleteLicence = model.Triathlon,
                UCILicence = model.UCI,
                BirthPlace = model.BirthPlace,
                IDNumber = model.IDNumber,
                MothersName = model.MothersName,
                UserName = model.FirstName.Replace(" ", "").RemoveDiacritics().ToLower() + "." + model.LastName.Replace(" ", "").RemoveDiacritics().ToLower() + "." + model.BirthDate.ToString("yyyyMMdd"),
                Address = address
            };

            IdentityResult identityResult = await CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                return identityResult;
            }
            
            if (IsEmailOnWildCardList(user.Email))
            {
                await VerifyUserAsync(user, loginUrl);
            }

            return identityResult;
        }
        public virtual async Task VerifyUserAsync(User user, string loginUrl)
        {
            if (!user.VerifiedByAdmin)
            {
                UnitOfWork.UserRepository.VerifyUser(user.Id);
                await EmailSender.SendAdminVerifiedEmailAsync(user.Email, user.FirstName, loginUrl);
                Logger.LogDebug($"{user.Email} verified.");
            }
        }

        protected internal virtual bool IsEmailOnWildCardList(string email)
        {
            var wildCardEmails = Configuration.GetSection("WildcardEmails").AsEnumerable();
            if(wildCardEmails == null || wildCardEmails.Count() == 0)
            {
                Logger.LogDebug("No wildcard e-mails are in the configuration.");
                return false;
            }

            return wildCardEmails.Any(x => x.Value == email);
        }

        public async Task<IdentityResult> SendForgotPasswordEmailAsync(User user, string host)
        {
            try
            {
                string passwordResetToken = await base.GeneratePasswordResetTokenAsync(user);
                string encodedToken = HttpUtility.UrlEncode(passwordResetToken);
                await EmailSender.SendForgotPasswordEmailAsync(user.Email, user.FirstName, encodedToken, user.Id.ToString(), host);
                return IdentityResult.Success;
            }
            catch (Exception e)
            {
                return IdentityResult.Failed(new IdentityError() { Description = e.Message });
            }
        }

        public async Task<UserModel> GetUserByNameAsync(string name)
        {
            User user = await base.FindByNameAsync(name);
            if (user == null)
            {
                return null;
            }

            UserModel response = await CreateUserModel(user);
            return response;
        }

        public async Task<UserModel> GetUserByIdAsync(string userId)
        {
            User user = await base.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            UserModel response = await CreateUserModel(user);
            return response;
        }

        public async Task<IEnumerable<UserModel>> ListUsersAsync()
        {
            List<UserModel> response = new List<UserModel>();
            foreach (var user in await UnitOfWork.UserRepository.ListAsync())
            {
                response.Add(await CreateUserModel(user));
            }

            return response;
        }

        public async Task UpdateAsync(UserModel model)
        {
            User user = await base.FindByIdAsync(model.Id.ToString());
            user.BirthDate = model.BirthDate;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.TShirtSize = model.TShirtSize.HasValue ? model.TShirtSize.Value : Size.S;
            user.VerifiedByAdmin = model.VerifiedByAdmin;
            user.AkeszNumber = model.AKESZ;
            user.OtprobaNumber = model.Otproba;
            user.UCILicence = model.UCI;
            user.TriathleteLicence = model.Triathlon;
            user.IDNumber = model.IDNumber;
            user.BirthPlace = model.BirthPlace;
            user.MothersName = model.MothersName;

            IdentityResult result = await base.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new IdentityException() { Errors = result.Errors };
            }
            
            Address address = await UnitOfWork.AddressRepository.GetByIDAsync(user.AddressId);
            address.HouseNumber = model.HouseNumber;
            address.Street = model.Street;
            address.ZipCode = model.ZipCode;
            address.City = model.City;
            address.Country = model.Country;
            await UnitOfWork.AddressRepository.UpdateAsync(address);

            UnitOfWork.Save();
            Logger.LogInformation($"User {user.Email} updated successfully.");
        }

        internal async Task<UserModel> CreateUserModel(User user)
        {
            try
            {
                UserModel response = new UserModel()
                {
                    BirthDate = user.BirthDate,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Gender = user.Gender,
                    Id = user.Id,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    TShirtSize = user.TShirtSize,
                    VerifiedByAdmin = user.VerifiedByAdmin,
                    AKESZ = user.AkeszNumber,
                    Otproba = user.OtprobaNumber,
                    Triathlon = user.TriathleteLicence,
                    UCI = user.UCILicence,
                    BirthPlace = user.BirthPlace,
                    MothersName = user.MothersName,
                    IDNumber = user.IDNumber,
                    IsPro = user.IsPro
                };

                Address usersAddress = await UnitOfWork.AddressRepository.GetByIDAsync(user.AddressId);
                if (usersAddress != null)
                {
                    response.ZipCode = usersAddress.ZipCode;
                    response.City = usersAddress.City;
                    response.HouseNumber = usersAddress.HouseNumber;
                    response.Street = usersAddress.Street;
                }

                return response;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Failed to convert user to user model. Id: {user.Id} / {user.Email}");
                throw;
            }
        }
    }
}
