using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class AddPointConsumptionViewModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }
        [Required]
        public string SelectedUserId { get; set; }
        public string Remark { get; set; }
        [Range(1, Double.MaxValue)]
        public int Amount { get; set; }

        public void Validate(ModelStateDictionary modelState, int currentPointsOfUser)
        {
            if(Amount <= 0)
            {
                modelState.AddModelError(nameof(Amount), "Amount cannot be 0.");
            }

            if(Amount > currentPointsOfUser)
            {
                modelState.AddModelError(nameof(Amount), $"The selected user does not have so many points. Maximum {currentPointsOfUser} is allowed.");
            }
        }
    }
}
