using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class User : IdentityUser<int>
    {
        public int AddressId { get; set; }
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        [PersonalData]
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public Size TShirtSize { get; set; }
        public int CollectedPoints { get; set; }
        public int ConsumedPoints { get; set; }
    }
}
