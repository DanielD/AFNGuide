using AfnGuideAPI.Models;

namespace AfnGuideAPI.Data
{
    [Serializable]
    public class ScheduleSearchResults
    {
        public int TotalCount { get; set; }
        public ScheduleSearchQuery Query { get; set; } = new();
        public List<Schedule> Schedules { get; set; } = new();
    }
}
