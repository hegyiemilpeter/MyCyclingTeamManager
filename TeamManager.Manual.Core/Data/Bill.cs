using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamManager.Manual.Data
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int Amount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Url { get; set; }
        public int Points { get; set; }
    }
}
