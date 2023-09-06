using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace AfnGuideAPI.Converters
{
    public partial class JavaScriptDateConverter : JsonConverter<DateTime>
    {
        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.Value!.ToString()!;
            var match = JSDate().Match(value);
            if (match.Success)
            {
                return new DateTime(
                    (int)match.Groups["Year"].Value.ToInt32()!,
                    (int)(match.Groups["Month"].Value.ToInt32()! + 1),
                    (int)match.Groups["Day"].Value.ToInt32()!,
                    (int)match.Groups["Hour"].Value.ToInt32()!,
                    (int)match.Groups["Minute"].Value.ToInt32()!,
                    (int)match.Groups["Second"].Value.ToInt32()!,
                    (int)match.Groups["Millisecond"].Value.ToInt32()!,
                    DateTimeKind.Utc);
            }
            else
            {
                return DateTime.Parse(value);
            }
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        [GeneratedRegex("(?<Year>[^,]+),(?<Month>[^,]+),(?<Day>[^,]+),(?<Hour>[^,]+),(?<Minute>[^,]+),(?<Second>[^,]+),(?<Millisecond>[^,]+)")]
        private static partial Regex JSDate();
    }
}
