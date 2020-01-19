using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string BirthPlace { get; set; }
        public string IDNumber { get; set; }
        public string MothersName { get; set; }
        public Gender Gender { get; set; }
        public Size TShirtSize { get; set; }
        public bool VerifiedByAdmin { get; set; }

        public string AkeszNumber { get; set; }
        public string UCILicence { get; set; }
        public string OtprobaNumber { get; set; }
        public string TriathleteLicence { get; set; }

        public bool HasEntryStatement { get; set; }
        public bool HasGDPRStatement { get; set; }
        public bool IsPro { get; set; }
    }
}
