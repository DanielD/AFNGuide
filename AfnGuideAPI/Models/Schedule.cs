using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int AfnId { get; set; }
        public int ChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime AirDateUTC { get; set; }
        public string? EpisodeTitle { get; set; }
        public int Duration { get; set; }
        public string? Genre { get; set; }
        public string? Rating { get; set; }
        public int? Year { get; set; }
        public bool IsPremiere { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        public virtual Channel? Channel { get; set; }
        public virtual Promo? Promo { get; set; }
    }
}
