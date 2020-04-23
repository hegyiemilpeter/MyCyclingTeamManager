using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "No token found.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "No userId found.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "The Confirm password field is required.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The Password and Confirm password should match.")]
        public string ConfirmPassword { get; set; }

        public override string ToString()
        {
            return $"Token: {Token}, UserId: {UserId}";
        }
    }
}
