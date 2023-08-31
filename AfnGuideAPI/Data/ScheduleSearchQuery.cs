namespace AfnGuideAPI.Data
{
    [Serializable]
    public class ScheduleSearchQuery
    {
        public string[] SearchWords { get; set; } = Array.Empty<string>();
        public int[]? Channels { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Rating { get; set; }
        public string? SearchField { get; set; }
        public string? SearchPhrase { get; set; }
        public string[]? UnwantedWords { get; set; }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(SearchWords);
            hash.Add(Channels);
            hash.Add(StartDate);
            hash.Add(EndDate);
            hash.Add(Rating);
            hash.Add(SearchField);
            hash.Add(SearchPhrase);
            hash.Add(UnwantedWords);
            return hash.ToHashCode();
        }
    }
}
