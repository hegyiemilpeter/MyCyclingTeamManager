using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class RegistrationViewModel : UserModel
    {
        #region Login data

        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The Password and Confirm password should match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        #endregion

        
    }
}
