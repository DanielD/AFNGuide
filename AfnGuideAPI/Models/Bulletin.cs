using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AfnGuideAPI.Models
{
    [Table("Bulletins")]
    public class Bulletin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }
    }
}
