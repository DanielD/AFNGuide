using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AfnGuideAPI.Models
{
    [Serializable]
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int AfnId { get; set; }
        public int ChannelId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime AirDateUTC { get; set; }
        public string? EpisodeTitle { get; set; }
        public int Duration { get; set; }
        public string? Genre { get; set; }
        public string? Rating { get; set; }
        public int? Year { get; set; }
        public bool IsPremiere { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }
        //[Column(TypeName = "decimal(2,1)")]
        //public decimal? RatingScore { get; set; } = 0.0M;
        //public string? Director { get; set; }
        //public DateTime? FirstAired { get; set; }

        public virtual Channel? Channel { get; set; }
        public virtual Promo? Promo { get; set; }

        public static Schedule Create(IDataRecord record)
        {
            return new Schedule
            {
                Id = (Guid)record["Id"],
                AfnId = (int)record["AfnId"],
                ChannelId = (int)record["ChannelId"],
                Title = record["Title"] as string,
                Description = record["Description"] as string,
                Category = record["Category"] as string,
                AirDateUTC = (DateTime)record["AirDateUTC"],
                EpisodeTitle = record["EpisodeTitle"] as string,
                Duration = (int)record["Duration"],
                Genre = record["Genre"] as string,
                Rating = record["Rating"] as string,
                Year = record["Year"] as int?,
                IsPremiere = (bool)record["IsPremiere"],
                CreatedOnUTC = (DateTime)record["CreatedOnUTC"],
                //RatingScore = record["RatingScore"] as decimal?,
                //Director = record["Director"] as string,
                //FirstAired = record["FirstAired"] as DateTime?
            };
        }
    }
}
