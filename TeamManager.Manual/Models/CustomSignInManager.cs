using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class CustomSignInManager : SignInManager<User>
    {
        private IConfiguration configuration;

        public CustomSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<User> confirmation, IConfiguration configuration) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            this.configuration = configuration;
        }

        public override Task<bool> CanSignInAsync(User user)
        {
            if (configuration.GetValue<bool>("UseAdminRestrictionForNewUsers") && !user.VerifiedByAdmin)
            {
                Logger.LogWarning($"{user.Email} cannot log in without admin verification.");
                return Task.FromResult(false);
            }

            return base.CanSignInAsync(user);
        }
    }
}
