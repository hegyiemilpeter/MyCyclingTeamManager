using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamManager.Manual.Core.Models;

namespace TeamManager.Manual.ViewModels
{
    public class AddResultViewModel
    {
        public AddResultViewModel()
        {
        }

        public AddResultViewModel(IList<RaceModel> races, int? selectedRaceId)
        {
            Races = races;
            SelectedRaceId = selectedRaceId;
        }

        public IList<RaceModel> Races { get; set; }
        [Required]
        public int? SelectedRaceId { get; set; }
        public int? AbsoluteResult { get; set; }
        public int? CategoryResult { get; set; }
        public bool IsTakePartAsStaff { get; set; }
        public IFormFile Image { get; set; }

        public IEnumerable<SelectListItem> GetRaceSelectList
        {
            get
            {
                List<SelectListItem> response = new List<SelectListItem>();

                if(Races != null)
                {
                    foreach (var race in Races)
                    {
                        response.Add(new SelectListItem()
                        {
                            Text = $"'{race.Date.Value.ToString("yy.MM.dd")} - {race.Name}",
                            Value = race.Id.ToString(),
                            Selected = SelectedRaceId.HasValue && SelectedRaceId.Value == race.Id
                        });
                    }
                }

                return response;
            }
        }

        public void Validate(ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if(!AbsoluteResult.HasValue && !CategoryResult.HasValue && !IsTakePartAsStaff)
            {
                modelState.AddModelError("", localizer["At least one result is required."]);
            }

            if((AbsoluteResult.HasValue || CategoryResult.HasValue) && Image == null)
            {
                modelState.AddModelError("Image", localizer["Image is required when you send an absolute or category result."]);
            }

            if ((AbsoluteResult.HasValue || CategoryResult.HasValue) && Image != null && (Image.ContentType != "image/jpeg" && Image.ContentType != "image/png"))
            {
                modelState.AddModelError("Image", localizer["PNG or JPG format is required for images."]);
            }
        }
    }
}
