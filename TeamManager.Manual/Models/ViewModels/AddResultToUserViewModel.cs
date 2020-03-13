using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class AddResultToUserViewModel : AddResultViewModel
    {
        public AddResultToUserViewModel()
        {
        }

        public AddResultToUserViewModel(IList<RaceModel> races, int? selectedRaceId, IEnumerable<UserModel> users, int? selectedUserId) : base(races, selectedRaceId)
        {
            Users = users;
            SelectedUserId = selectedUserId;
        }

        public IEnumerable<UserModel> Users { get; set; }

        [Required]
        public int? SelectedUserId { get; set; }

        public new void Validate(ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if (!AbsoluteResult.HasValue && !CategoryResult.HasValue && !IsTakePartAsStaff)
            {
                modelState.AddModelError("", localizer["At least one result is required."]);
            }

            if (!SelectedUserId.HasValue)
            {
                modelState.AddModelError("", localizer["User is required."]);
            }
        }

        public IEnumerable<SelectListItem> GetUserList
        {
            get
            {
                List<SelectListItem> response = new List<SelectListItem>();

                if (Races != null)
                {
                    foreach (var user in Users)
                    {
                        response.Add(new SelectListItem()
                        {
                            Text = user.FullName,
                            Value = user.Id.ToString(),
                            Selected = SelectedUserId.HasValue && SelectedUserId.Value == user.Id
                        });
                    }
                }

                return response;
            }
        }
    }
}
