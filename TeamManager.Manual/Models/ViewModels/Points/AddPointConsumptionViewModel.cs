using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class AddPointConsumptionViewModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }
        [Required(ErrorMessage = "The user is required.")]
        public string SelectedUserId { get; set; }
        public string Remark { get; set; }
        [Range(1, Double.MaxValue, ErrorMessage = "Must be between 1 and 1000.")]
        public int Amount { get; set; }

        public void Validate(ModelStateDictionary modelState, int currentPointsOfUser, IStringLocalizer localizer)
        {
            if(Amount > currentPointsOfUser)
            {
                modelState.AddModelError(nameof(Amount), string.Format(localizer["The selected user does not have so many points. Maximum {0} is allowed."], currentPointsOfUser));
            }
        }
    }
}
