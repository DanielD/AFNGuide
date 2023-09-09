using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("RadioChannels")]
    public class RadioChannel
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? TagLine { get; set; }
        public int? Duration { get; set; }
        public byte[]? LogoData { get; set; }
        public byte[]? LogoSlideData { get; set; }
        public byte[]? LogoThumbnailData { get; set; }
    }

    public class JsonRadioChannel
    {
        [JsonProperty("a")]
        public int Id { get; set; }
        [JsonProperty("b")]
        public string? Name { get; set; }
        [JsonProperty("c")]
        public string? Description { get; set; }
        [JsonProperty("d")]
        public string? TagLine { get; set; }
        [JsonProperty("e")]
        public string? Logo { get; set; }
        [JsonProperty("f")]
        public string? LogoSlide { get; set; }
        [JsonProperty("g")]
        public string? LogoThumbnail { get; set; }
        [JsonProperty("h")]
        public int? Duration { get; set; }
    }
}
