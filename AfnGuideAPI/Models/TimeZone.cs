using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AfnGuideAPI.Models
{
    [Table("TimeZones")]
    public class TimeZone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public bool ObservesDST { get; set; }
        public int Offset { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime? DSTStartsOn { get; set; }
        public DateTime? DSTEndsOn { get; set; }
        public bool IsBackwards { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? ModifiedOnUTC { get; set; }

        public ICollection<ChannelTimeZone> ChannelTimeZones { get; set; } = new List<ChannelTimeZone>();

        public TimeZone()
        {
        }

        public TimeZone(int id, string name, string? abbreviation, double offset, bool observesDST, DateTime? dstStartsOn = null, DateTime? dstEndsOn = null, bool isBackwards = false)
        {
            Id = id;
            Name = name;
            Abbreviation = abbreviation;
            ObservesDST = observesDST;
            Offset = (int)(offset * 60);
            DSTStartsOn = dstStartsOn;
            DSTEndsOn = dstEndsOn;
            IsBackwards = isBackwards;
            CreatedOnUTC = DateTime.UtcNow;
        }

        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow.AddMinutes(GetCurrentOffset());
        }

        public int GetCurrentOffset()
        {
            if (!ObservesDST)
            {
                return Offset;
            }

            var now = DateTime.UtcNow;
            DateTime? dstStart = null, dstEnd = null;
            
            if (!IsBackwards)
            {
                dstStart = new DateTime(now.Year, DSTStartsOn!.Value.Month, DSTStartsOn!.Value.Day, DSTStartsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);
                dstEnd = new DateTime(now.Year, DSTEndsOn!.Value.Month, DSTEndsOn!.Value.Day, DSTEndsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);

                if (now >= dstStart && now < dstEnd)
                {
                    return Offset + 60;
                }
            }
            else
            {
                if (now.Month < DSTStartsOn!.Value.Month)
                {
                    dstStart = new DateTime(now.Year - 1, DSTStartsOn!.Value.Month, DSTStartsOn!.Value.Day, DSTStartsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);
                    dstEnd = new DateTime(now.Year, DSTEndsOn!.Value.Month, DSTEndsOn!.Value.Day, DSTEndsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);
                }
                else if (now.Month > DSTEndsOn!.Value.Month)
                {
                    dstStart = new DateTime(now.Year, DSTStartsOn!.Value.Month, DSTStartsOn!.Value.Day, DSTStartsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);
                    dstEnd = new DateTime(now.Year + 1, DSTEndsOn!.Value.Month, DSTEndsOn!.Value.Day, DSTEndsOn!.Value.Hour, 0, 0, DateTimeKind.Utc);
                }

                if (dstStart.HasValue && dstEnd.HasValue)
                {
                    if (now >= dstStart && now < dstEnd)
                    {
                        return Offset + 60;
                    }
                }
            }

            return Offset;
        }

        public override string ToString()
        {
            var offset = GetCurrentOffset();
            var time = "";
            if (offset < 0)
            {
                time = $"{offset / 60:00}:{offset % 60:00}";
            }
            else if (offset > 0)
            {
                time = $"+{offset / 60:00}:{offset % 60:00}";
            }

            return $"(GMT{time}) {Name}{((!ObservesDST) ? " (no DST)" : "")}";
        }
    }
}
