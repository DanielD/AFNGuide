namespace AfnGuideAPI.ViewModels
{
    public class SportsCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public SportsCategory() { }

        public SportsCategory(Models.SportsCategory sportsCategory)
        {
            Id = sportsCategory.Id;
            Name = sportsCategory.Name;
        }
    }
}
