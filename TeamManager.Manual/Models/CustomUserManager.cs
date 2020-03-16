﻿using Diacritics.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TeamManager.Manual.Core.Interfaces;
using TeamManager.Manual.Data;
using TeamManager.Manual.Models.Exceptions;
using TeamManager.Manual.Models.Interfaces;

namespace TeamManager.Manual.Models
{
    public class CustomUserManager : UserManager<User>
    {
        private TeamManagerDbContext DbContext { get; }
        private IConfiguration Configuration { get; }
        private IEmailSender EmailSender { get; }

        public CustomUserManager(TeamManagerDbContext dbContext, IConfiguration configuration, IEmailSender emailSender, IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            DbContext = dbContext;
            Configuration = configuration;
            EmailSender = emailSender;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password, Address address, string loginUrl)
        {
            user.UserName = user.FirstName.Replace(" ", "").RemoveDiacritics().ToLower() + "." + user.LastName.Replace(" ", "").RemoveDiacritics().ToLower() + "." + user.BirthDate.ToString("yyyyMMdd");

            IdentityResult identityResult = await base.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                return identityResult;
            }
            
            if (IsEmailOnWildCardList(user.Email))
            {
                await VerifyUserAsync(user, loginUrl);
            }

            try
            {
                DbContext.Addresses.Add(address);
                await DbContext.SaveChangesAsync();

                user.AddressId = address.Id;
                DbContext.Entry(user).State = EntityState.Modified;

                await DbContext.SaveChangesAsync();
                return identityResult;
            }
            catch
            {
                identityResult.Errors.Append(new IdentityError() { Description = $"Update with address and/or identification number failed for {user.FirstName} {user.LastName}." });
                return identityResult;
            }
        }
        public async Task VerifyUserAsync(User user, string loginUrl)
        {
            if (!user.VerifiedByAdmin)
            {
                user.VerifiedByAdmin = true;

                DbContext.Entry(user).State = EntityState.Modified;
                await DbContext.SaveChangesAsync();

                Logger.LogDebug($"{user.Email} verified.");

                await EmailSender.SendAdminVerifiedEmailAsync(user.Email, user.FirstName, loginUrl);
            }
        }

        private bool IsEmailOnWildCardList(string email)
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

            UserModel response = CreateUserModel(user);
            return response;
        }

        public async Task<UserModel> GetUserByIdAsync(string userId)
        {
            User user = await base.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            UserModel response = CreateUserModel(user);
            return response;
        }

        public async Task<IEnumerable<UserModel>> ListUsersAsync()
        {
            List<UserModel> response = new List<UserModel>();
            foreach (var user in await DbContext.Users.ToListAsync())
            {
                response.Add(CreateUserModel(user));
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

            await DbContext.SaveChangesAsync();

            Address address = DbContext.Addresses.SingleOrDefault(x => x.Id == user.AddressId);
            address.HouseNumber = model.HouseNumber;
            address.Street = model.Street;
            address.ZipCode = model.ZipCode;
            address.City = model.City;
            address.Country = model.Country;
            DbContext.Addresses.Update(address);

            await DbContext.SaveChangesAsync();
            Logger.LogInformation($"User {user.Email} updated successfully.");
        }

        internal UserModel CreateUserModel(User user)
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

                Address usersAddress = DbContext.Addresses.SingleOrDefault(x => x.Id == user.AddressId);
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
