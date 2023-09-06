using AfnGuideAPI.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace AfnGuideAPI.Models
{
    [Table("SportsSchedules")]
    public class SportsSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 
        public int AfnId { get; set; }
        public int ChannelId { get; set; }
        public string? SportName { get; set; }
        public bool IsTapeDelayed { get; set; }
        public bool IsLive { get; set; }
        public DateTime AirDateUTC { get; set; }
        public int? SportsNetworkId { get; set; }
        public int? SportsCategoryId { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }

        public virtual Channel? Channel { get; set; }
        public virtual SportsNetwork? SportsNetwork { get; set; }
        public virtual SportsCategory? SportsCategory { get; set; }

        public static SportsSchedule Create(IDataRecord record)
        {
            return new SportsSchedule
            {
                Id = (Guid)record["Id"],
                AfnId = (int)record["AfnId"],
                ChannelId = (int)record["ChannelId"],
                SportName = record["SportName"] as string,
                SportsCategoryId = (int)record["SportsCategoryId"],
                AirDateUTC = (DateTime)record["AirDateUTC"],
                IsTapeDelayed = (bool)record["IsTapeDelayed"],
                IsLive = (bool)record["IsLive"],
                //SportsNetworkId = (int)record["SportsNetworkId"],
                CreatedOnUTC = (DateTime)record["CreatedOnUTC"],
            };
        }
    }

    public class JsonSportsSchedule
    {
        [JsonProperty("d")]
        public List<JsonScportsScheduleItem>? SportsScheduleItems { get; set; }
    }

    public class JsonScportsScheduleItem
    {
        [JsonProperty("a")]
        public int AfnId { get; set; }//a
        [JsonProperty("b")]
        public int ChannelId { get; set; }//b
        [JsonProperty("e")]
        public string? SportCategory { get; set; }//e
        [JsonProperty("f")]
        public string? SportName { get; set; }//f
        [JsonProperty("k")]
        public bool IsTapeDelayed { get; set; }//k
        [JsonProperty("l")]
        public bool IsLive { get; set; }//l
        [JsonProperty("m")]
        [JsonConverter(typeof(JavaScriptDateConverter))]
        public DateTime AirDateUTC { get; set; }//m
    }
}
