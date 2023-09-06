#nullable disable

namespace AfnGuideAPI.Data.SearchEngines
{
    [Serializable]
    public class SearchResults<TSchedule>
        : ISearchResults<TSchedule>
        where TSchedule : class
    {
        public int TotalCount { get; set; }
        public ISearchQuery Query { get; set; }
        public List<TSchedule> Schedules { get; set; } = new();
    }
}
