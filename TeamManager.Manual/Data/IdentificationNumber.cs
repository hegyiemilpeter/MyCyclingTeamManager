using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class IdentificationNumber
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public IdentificationNumberType Type { get; set; }
        public string Value { get; set; }
    }
}
