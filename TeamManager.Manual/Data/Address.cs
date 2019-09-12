using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ZipCode { get; set; }
    }
}
