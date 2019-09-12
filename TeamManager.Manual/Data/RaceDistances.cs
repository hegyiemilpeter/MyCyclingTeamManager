using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class RaceDistances
    {
        [Key]
        public int Id { get; set; }
        public int RaceId { get; set; }
        
        /// <summary>
        /// Race distance in km-s.
        /// </summary>
        public int Distance { get; set; }
    }
}
