using Diacritics.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TeamManager.Manual.Data;

namespace TeamManager.Manual.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "The Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int Id { get; set; }

        public bool VerifiedByAdmin { get; set; }

        #region Personal data

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Birth place is required.")]
        public string BirthPlace { get; set; }
        
        [Required(ErrorMessage = "ID number is required.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Mothers name is required.")]
        public string MothersName { get; set; }

        [Required(ErrorMessage = "The phone number is required.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "T-shirt size is required.")]
        public Size? TShirtSize { get; set; }

        public string FullName => FirstName + " " + LastName;

        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
                    return string.Empty;
                return $"{FirstName.Replace(" ", "").RemoveDiacritics().ToLower()}.{LastName.Replace(" ", "").RemoveDiacritics().ToLower()}.{BirthDate.ToString("yyyyMMdd")}";
            }
        }

        #endregion

        #region Address

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        [RegularExpression("[0-9]{4}", ErrorMessage = "As a zip code enter only 4 numbers.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "House number is required.")]
        public string HouseNumber { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        public string Street { get; set; }

        public string Country => "Hungary";

        #endregion

        #region Identifiers

        public string AKESZ { get; set; }

        public string Otproba { get; set; }

        public string UCI { get; set; }

        public string Triathlon { get; set; }

        #endregion

        public void Validate(ModelStateDictionary modelState, IStringLocalizer localizer)
        {
            if (BirthDate > (new DateTime(DateTime.Today.Year, 12, 31).AddYears(-7)))
            {
                modelState.AddModelError(nameof(BirthDate), localizer["You have to be at least 6 years old to be part of the system."]);
            }

        }
    }
}
