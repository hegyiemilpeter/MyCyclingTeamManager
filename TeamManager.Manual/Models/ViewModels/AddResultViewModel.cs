using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class AddResultViewModel
    {
        public IList<RaceModel> Races { get; set; }
        [Required]
        public int? SelectedRaceId { get; set; }
        public int? AbsoluteResult { get; set; }
        public int? CategoryResult { get; set; }
        public bool IsTakePartAsStaff { get; set; }
        public bool IsTakePartAsDriver { get; set; }

        public IEnumerable<SelectListItem> GetRaceSelectList
        {
            get
            {
                List<SelectListItem> response = new List<SelectListItem>();
                response.Add(new SelectListItem() { Text = "Select a race", Value = null });

                if(Races != null)
                {
                    foreach (var race in Races)
                    {
                        response.Add(new SelectListItem()
                        {
                            Text = race.Name,
                            Value = race.Id.ToString(),
                            Selected = SelectedRaceId.HasValue && SelectedRaceId.Value == race.Id
                        });
                    }
                }

                return response;
            }
        }

        public void Validate(ModelStateDictionary modelState)
        {
            if(!AbsoluteResult.HasValue && !CategoryResult.HasValue && !IsTakePartAsDriver && !IsTakePartAsStaff)
            {
                modelState.AddModelError("", "At least one result is required.");
            }
        }
    }
}
