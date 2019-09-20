using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models.ViewModels
{
    public class RaceModel : Race
    {
        public IList<int> DistanceLengths { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            if (DistanceLengths == null || DistanceLengths.Count == 0)
            {
                modelState.AddModelError(nameof(DistanceLengths), "At least one distance is required.");
            }

            if (modelState.GetFieldValidationState(nameof(Date)) == ModelValidationState.Valid &&
                modelState.GetFieldValidationState(nameof(EntryDeadline)) == ModelValidationState.Valid
                && Date.Value < EntryDeadline.Value)
            {
                modelState.AddModelError(string.Empty, "The entry deadline must be later than the race date.");
            }
        }
    }
}
