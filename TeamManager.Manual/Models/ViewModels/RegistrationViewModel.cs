using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class RegistrationViewModel : UserModel
    {
        #region Login data

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        #endregion

        
    }
}
