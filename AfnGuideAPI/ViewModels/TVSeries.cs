namespace AfnGuideAPI.ViewModels
{
    public class TVSeries
    {
        public string? Title { get; set; }
        public int? Season { get; set; }
        public DateTime? StartDate { get; set; }
        public int? PremiereType { get; set; }
        public int? ChannelId { get; set; }
        public bool IsSplit { get; set; }
    }
}
