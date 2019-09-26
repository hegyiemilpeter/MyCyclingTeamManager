using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int Id { get; set; }

        #region Personal data

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        public Gender? Gender { get; set; }

        [Required]
        public Size? TShirtSize { get; set; }

        #endregion

        #region Address

        [Required]
        public string City { get; set; }

        [Required]
        [RegularExpression("[0-9]{4}", ErrorMessage = "As a zip code enter only 4 numbers.")]
        public string ZipCode { get; set; }

        [Required]
        public string HouseNumber { get; set; }

        [Required]
        public string Street { get; set; }

        public string Country => "Hungary";

        #endregion

        #region Identifiers

        public string AKESZ { get; set; }

        public string Otproba { get; set; }

        public string UCI { get; set; }

        public string Triathlon { get; set; }

        #endregion

        public void Validate(ModelStateDictionary modelState)
        {
            if (BirthDate > (new DateTime(DateTime.Today.Year, 12, 31).AddYears(-7)))
            {
                modelState.AddModelError(nameof(BirthDate), "You have to be at least 6 years old to be part of the system.");
            }

        }
    }
}
