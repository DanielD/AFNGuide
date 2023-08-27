using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("Promos")]
    public class Promo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = "";
        public int? AfnId { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public int? Duration { get; set; }
        public bool IsPromoB { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedOnUTC { get; set; }

        public virtual Schedule? Schedule { get; set; }
    }
}
