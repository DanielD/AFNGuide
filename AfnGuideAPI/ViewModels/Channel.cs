namespace AfnGuideAPI.ViewModels
{
    public class Channel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Abbreviation { get; set; }
        public int? ChannelNumber { get; set; }
        public string? Color { get; set; }
        public string? Image { get; set; }
        public bool IsSplit { get; set; }

        public ICollection<ChannelTimeZone> ChannelTimeZones { get; set; } = new List<ChannelTimeZone>();
    }

    public class ChannelTimeZone
    {
        public int TimeZoneId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
