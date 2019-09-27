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
using TeamManager.Manual.Models.Exceptions;

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
            foreach (var user in _dbContext.Users.ToList())
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

            IdentityResult result = await base.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new IdentityException() { Errors = result.Errors };
            }

            foreach (var item in _dbContext.IdentificationNumbers.Where(x => x.UserId == user.Id).ToList())
            {
                _dbContext.IdentificationNumbers.Remove(item);
            }

            await _dbContext.SaveChangesAsync();

            if (!string.IsNullOrEmpty(model.AKESZ))
            {
                _dbContext.IdentificationNumbers.Add(
                    new IdentificationNumber()
                    {
                        Type = IdentificationNumberType.AKESZ,
                        UserId = user.Id,
                        Value = model.AKESZ
                    });
            }

            if (!string.IsNullOrEmpty(model.UCI))
            {
                _dbContext.IdentificationNumbers.Add(
                    new IdentificationNumber()
                    {
                        Type = IdentificationNumberType.UCILicence,
                        UserId = user.Id,
                        Value = model.UCI
                    });
            }

            if (!string.IsNullOrEmpty(model.Triathlon))
            {
                _dbContext.IdentificationNumbers.Add(
                    new IdentificationNumber()
                    {
                        Type = IdentificationNumberType.TriathleteLicence,
                        UserId = user.Id,
                        Value = model.Triathlon
                    });
            }

            if (!string.IsNullOrEmpty(model.Otproba))
            {
                _dbContext.IdentificationNumbers.Add(
                    new IdentificationNumber()
                    {
                        Type = IdentificationNumberType.OtProba,
                        UserId = user.Id,
                        Value = model.Otproba
                    });
            }

            Address address = _dbContext.Addresses.SingleOrDefault(x => x.Id == user.AddressId);
            address.HouseNumber = model.HouseNumber;
            address.Street = model.Street;
            address.ZipCode = model.ZipCode;
            address.City = model.City;
            address.Country = model.Country;
            _dbContext.Addresses.Update(address);

            await _dbContext.SaveChangesAsync();
        }

        internal UserModel CreateUserModel(User user)
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
                TShirtSize = user.TShirtSize
            };

            Address usersAddress = _dbContext.Addresses.SingleOrDefault(x => x.Id == user.AddressId);
            if (usersAddress != null)
            {
                response.ZipCode = usersAddress.ZipCode;
                response.City = usersAddress.City;
                response.HouseNumber = usersAddress.HouseNumber;
                response.Street = usersAddress.Street;
            }

            IEnumerable<IdentificationNumber> identificationNumbers = _dbContext.IdentificationNumbers.Where(x => x.UserId == user.Id);
            if (identificationNumbers != null && identificationNumbers.Count() > 0)
            {
                foreach (var number in identificationNumbers)
                {
                    switch (number.Type)
                    {
                        case IdentificationNumberType.AKESZ:
                            response.AKESZ = number.Value;
                            break;
                        case IdentificationNumberType.UCILicence:
                            response.UCI = number.Value;
                            break;
                        case IdentificationNumberType.OtProba:
                            response.Otproba = number.Value;
                            break;
                        case IdentificationNumberType.TriathleteLicence:
                            response.Triathlon = number.Value;
                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }
    }
}
