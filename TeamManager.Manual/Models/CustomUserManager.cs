using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class CustomUserManager : UserManager<User>
    {
        private TeamManagerDbContext _dbContext { get; }

        public CustomUserManager(TeamManagerDbContext dbContext, IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<IdentityResult> CreateAsync(User user, string password, Address address, Dictionary<IdentificationNumberType, string> identifiers)
        {
            user.UserName = user.FirstName.ToLower() + "." + user.LastName.ToLower() + "." + user.BirthDate.ToString("yyyyMMdd");

            IdentityResult identityResult = await base.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                return identityResult;
            }

            try
            {
                _dbContext.Addresses.Add(address);
                await _dbContext.SaveChangesAsync();

                user.AddressId = address.Id;
                _dbContext.Entry(user).State = EntityState.Modified;

                if(identifiers != null && identifiers.Count > 0)
                {
                    foreach (var identifier in identifiers)
                    {
                        IdentificationNumber identificationNumber = new IdentificationNumber()
                        {
                            Type = identifier.Key,
                            Value = identifier.Value,
                            UserId = user.Id
                        };

                        _dbContext.IdentificationNumbers.Add(identificationNumber);
                    }
                }

                await _dbContext.SaveChangesAsync();
                return identityResult;
            }
            catch
            {
                identityResult.Errors.Append(new IdentityError() { Description = $"Update with address and/or identification number failed for {user.FirstName} {user.LastName}." });
                return identityResult;
            }
        }
    }
}
