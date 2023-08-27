namespace AfnGuideAPI.ViewModels
{
    public class Bulletin
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

        public Bulletin(Models.Bulletin bulletin)
        {
            Id = bulletin.Id;
            Title = bulletin.Title;
            Content = bulletin.Content;
        }

        public Bulletin()
        {
        }
    }
}
