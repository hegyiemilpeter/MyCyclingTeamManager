using System.ComponentModel.DataAnnotations;

namespace TeamManager.Manual.Data
{
    public class RaceDistance
    {
        [Key]
        public int Id { get; set; }
        public int RaceId { get; set; }
        public virtual Race Race { get; set; }

        /// <summary>
        /// Race distance in km-s.
        /// </summary>
        public int Distance { get; set; }
    }
}
