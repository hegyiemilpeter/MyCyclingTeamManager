using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "The Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
        public int ValidationNumber { get; set; }

        public void Validate(byte[] sessionValidation, ModelStateDictionary modelErrors, IStringLocalizer localizer)
        {
            if(sessionValidation == null || sessionValidation.Length != 1 || ValidationNumber != sessionValidation[0])
            {
                modelErrors.AddModelError(nameof(ValidationNumber), localizer["Validation number does not match."]);
            }
        }

        public override string ToString()
        {
            return $"Email: {Email}, Message: {Message}";
        }
    }
}
