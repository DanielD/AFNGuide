namespace AfnGuideAPI.Data.SearchEngines.SportsSchedules
{
    public class SearchQuery : ISearchQuery
    {
        public int[]? Channels { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string[]? SearchWords { get; set; }
        public string? SearchPhrase { get; set; }
        public string[]? UnwantedWords { get; set; }
        public int? SportCategoryId { get; set; }
        public bool IsLive { get; set; }
    }
}
