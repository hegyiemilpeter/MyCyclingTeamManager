using System;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class Rider
    {
        [Key]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [DataType(DataType.Password)]
        public string HashedPassword { get; set; }
        public Gender Gender { get; set; }
        public Size TShirtSize { get; set; }
        public int CollectedPoints { get; set; }
        public int ConsumedPoints { get; set; }

    }
}
