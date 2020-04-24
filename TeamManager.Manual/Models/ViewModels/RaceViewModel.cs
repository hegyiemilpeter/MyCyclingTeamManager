using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Linq;
using TeamManager.Manual.Core.Models;
using TeamManager.Manual.Web.Models.ViewModels;

namespace TeamManager.Manual.ViewModels
{
    public class RaceViewModel : RaceModel
    {
        public RaceViewModel()
        {
        }


        public RaceViewModel(RaceModel model)
        {
            if(model != null)
                ModelToViewModelConverter.Convert(model, this);
        }

        public string DistancesString
        {
            get
            {
                string distancesString = null;
                for (int i = 0; i < DistanceLengths.Count(); i++)
                {
                    distancesString += i == DistanceLengths.Count() - 1 ? DistanceLengths[i] + " km " : DistanceLengths[i] + " km, ";
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

        public void Validate(ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if (DistanceLengths == null || DistanceLengths.Count() == 0 || DistanceLengths.All(x => string.IsNullOrWhiteSpace(x)))
            {
                modelState.AddModelError(nameof(DistanceLengths), localizer["At least one distance is required."]);
            }
            else if(DistanceLengths.Any(d => Double.Parse(d, CultureInfo.InvariantCulture) <= 0))
            {
                modelState.AddModelError(nameof(DistanceLengths), localizer["All distances should be greater than 0."]);
            }

            if (modelState.GetFieldValidationState(nameof(Date)) == ModelValidationState.Valid &&
                modelState.GetFieldValidationState(nameof(EntryDeadline)) == ModelValidationState.Valid
                && (EntryDeadline.HasValue &&  Date.Value < EntryDeadline.Value))
            {
                modelState.AddModelError(string.Empty, localizer["The entry deadline must be sooner than the race date."]);
            }
        }
    }
}
