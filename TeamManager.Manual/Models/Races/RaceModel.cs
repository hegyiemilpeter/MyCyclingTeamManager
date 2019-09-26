using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class RaceModel : Race
    {
        public IList<int> DistanceLengths { get; set; }

        public string DistancesString
        {
            get
            {
                string distancesString = null;
                for (int i = 0; i < DistanceLengths.Count; i++)
                {
                    distancesString += i == DistanceLengths.Count - 1 ? DistanceLengths[i] + " km " : DistanceLengths[i] + " km, ";
                }

                return distancesString;
            }
        }

        public string CityString
        {
            get
            {
                string cityString = City;
                if (!string.IsNullOrWhiteSpace(Country))
                {
                    cityString += $" ({Country})";
                }

                return cityString;
            }
        }

        public void Validate(ModelStateDictionary modelState)
        {
            if (DistanceLengths == null || DistanceLengths.Count == 0)
            {
                modelState.AddModelError(nameof(DistanceLengths), "At least one distance is required.");
            }
            else if(DistanceLengths.Any(d => d <= 0))
            {
                modelState.AddModelError(nameof(DistanceLengths), "All distances should be greater than 0.");
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
