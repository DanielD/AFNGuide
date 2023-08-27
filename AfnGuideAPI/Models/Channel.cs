using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("Channels")]
    public class Channel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Abbreviation { get; set; }
        public int? ChannelNumber { get; set; }
        public string? Color { get; set; }
        public string? Image { get; set; }
        public bool IsSplit { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
