using AfnGuideAPI.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("TVSeries")]
    public class TVSeries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ChannelId { get; set; }
        public bool IsSplit { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Season { get; set; }
        public int? PremiereType { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }

        public Channel? Channel { get; set; }
    }

    public class JsonTVSeries
    {
        [JsonProperty("a")]
        public string? Name { get; set; }
        [JsonConverter(typeof(JavaScriptDateConverter))]
        [JsonProperty("b")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("c")]
        public int? Season { get; set; }
        [JsonProperty("d")]
        public string? ChannelName { get; set; }
        [JsonProperty("e")]
        public string? PremiereType { get; set; }
    }
}
