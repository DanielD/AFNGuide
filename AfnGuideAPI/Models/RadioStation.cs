using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("RadioStations")]
    public class RadioStation
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Schedules { get; set; }
        public byte[]? LogoData { get; set; }
    }

    public class JsonRadioStation
    {
        [JsonProperty("a")]
        public int Id { get; set; }
        [JsonProperty("b")]
        public string? Name { get; set; }
        [JsonProperty("c")]
        public string? Description { get; set; }
        [JsonProperty("d")]
        public string? Logo { get; set; }
        [JsonProperty("f")]
        public List<JsonRadioStationSchedule>? Schedules { get; set; }
    }

    public class JsonRadioStationSchedule
    {
        [JsonProperty("a")]
        public string? Day { get; set; }
        [JsonProperty("b")]
        public string? Schedule { get; set; }
    }
}
