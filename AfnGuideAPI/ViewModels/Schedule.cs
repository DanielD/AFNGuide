namespace AfnGuideAPI.ViewModels
{
    public class Schedule
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime AirDateLocal { get; set; }
        public string? EpisodeTitle { get; set; }
        public int Duration { get; set; }
        public string? Genre { get; set; }
        public string? Rating { get; set; }
        public int? Year { get; set; }
        public bool IsPremiere { get; set; }
        //public decimal? RatingScore { get; set; } = 0.0M;
        //public string? Director { get; set; }
        //public DateTime? FirstAired { get; set; }
    }
}
