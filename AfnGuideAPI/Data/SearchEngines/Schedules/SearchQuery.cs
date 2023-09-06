namespace AfnGuideAPI.Data.SearchEngines.Schedules
{
    [Serializable]
    public class SearchQuery : ISearchQuery
    {
        public string[] SearchWords { get; set; } = Array.Empty<string>();
        public int[]? Channels { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Rating { get; set; }
        public string? SearchField { get; set; }
        public string? SearchPhrase { get; set; }
        public string[]? UnwantedWords { get; set; }
    }
}
