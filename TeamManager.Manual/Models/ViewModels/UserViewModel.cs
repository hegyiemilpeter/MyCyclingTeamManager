using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.ViewModels
{
    public class UserViewModel : UserModel
    {
        public void Validate(ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if (BirthDate > (new DateTime(DateTime.Today.Year, 12, 31).AddYears(-7)))
            {
                modelState.AddModelError(nameof(BirthDate), localizer["You have to be at least 6 years old to be part of the system."]);
            }

        }
    }
}
