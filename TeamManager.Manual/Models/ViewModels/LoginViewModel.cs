﻿using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
