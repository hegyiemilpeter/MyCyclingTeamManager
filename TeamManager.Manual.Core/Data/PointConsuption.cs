using System;
using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class PointConsuption
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        
        public User User { get; set; }


    }
}
