namespace AfnGuideAPI.ViewModels
{
    public class SportsSchedule
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string? SportName { get; set; }
        public bool IsTapeDelayed { get; set; }
        public bool IsLive { get; set; }
        public DateTime AirDateLocal { get; set; }
        public int? SportsCategoryId { get; set; }

        public string? LiveDisplay
        {
            get
            {
                if (IsLive)
                {
                    return "Live";
                }
                else if (IsTapeDelayed)
                {
                    return "Tape Delayed";
                }
                else
                {
                    return "TBD";
                }
            }
        }
    }
}
