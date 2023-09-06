namespace AfnGuideAPI.ViewModels
{
    public class SearchResult<T> where T : class
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public List<T> Schedules { get; set; } = new List<T>();
    }
}
