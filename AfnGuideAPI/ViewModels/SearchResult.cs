namespace AfnGuideAPI.ViewModels
{
    public class SearchResult
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
