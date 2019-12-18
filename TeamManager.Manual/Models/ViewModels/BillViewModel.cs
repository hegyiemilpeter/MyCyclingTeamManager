using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Models.ViewModels
{
    public class BillViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Amount should be greater than zero.")]
        public int Amount { get; set; }
        
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "The Purchase date field is required.")]
        public DateTime? PurchaseDate { get; set; }
        [Required(ErrorMessage = "The Image field is required.")]
        public IFormFile Image { get; set; }
    }
}
