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
        public bool IsSports { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<ChannelTimeZone> ChannelTimeZones { get; set; } = new List<ChannelTimeZone>();
        public ICollection<SportsSchedule> SportsSchedules { get; set; } = new List<SportsSchedule>();
        public ICollection<TVSeries> TVSeries { get; set; } = new List<TVSeries>();
    }

    [Table("ChannelTimeZones")]
    public class ChannelTimeZone
    {
        [Key]
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public int TimeZoneId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }

        public Channel Channel { get; set; } = null!;
        public TimeZone TimeZone { get; set; } = null!;
    }

    public class JsonChannel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Abbreviation { get; set; }
        public int? ChannelNumber { get; set; }
        public string? Color { get; set; }
        public string? Image { get; set; }
        public bool IsSplit { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
