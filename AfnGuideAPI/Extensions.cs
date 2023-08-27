using AfnGuideAPI.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AfnGuideAPI
{
    internal static partial class Extensions
    {
        public static string ToTitleCase(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        public static bool IsEmpty(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool ToBoolean(this string? value)
        {
            return value?.ToLower() switch
            {
                "true" => true,
                "false" => false,
                "1" => true,
                "0" => false,
                _ => false
            };
        }

        public static string? ToDateString(this string? value)
        {
            value ??= string.Empty;

            return RssDateTimeString().Replace(value, "${Month}/${Day}/${Year} ${Hour}:${Minute}:${Second}");
        }

        public static DateTime? ToDateTime(this string? value)
        {
            return DateTime.TryParse(value, out var result) ? result : default;
        }

        public static int? ToInt32(this string? value)
        {
            return int.TryParse(value, out var result) ? result : default;
        }

        [GeneratedRegex("(?<Year>[^\\-]+)-(?<Month>[^\\-]+)-(?<Day>[^T]+)T(?<Hour>[^\\:]+):(?<Minute>[^\\:]+):(?<Second>[^\\s]+)\\sGMT")]
        private static partial Regex RssDateTimeString();

        public static HttpClient AddBrowserHeader(this HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
            return client;
        }

        public static void Add<T>(this MultipartFormDataContent form, string name, T value)
        {
            form.Add(new StringContent($"{value}"), name);
        }

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            var value = configuration.GetValue<T>(key);
            return value == null 
                ? throw new InvalidOperationException($"Configuration value for {key} is required") 
                : value;
        }

        public static T GetRequiredSection<T>(this IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key);
            if (section == null)
            {
                throw new InvalidOperationException($"Configuration section for {key} is required");
            }
            else
            {
                var typedSection = section.Get<T>();
                if (typedSection == null)
                {
                    throw new InvalidOperationException($"Configuration section for {key} is required");
                }
                else
                {
                    return typedSection;
                }
            }
        }

        public static Task<List<Schedule>> FindByTitleAsync(this DbSet<Schedule> schedules, string title, CancellationToken cancellationToken = default)
        {
            title = title.Replace(" ", "");
            return schedules
                .Include(s => s.Channel)
                .Where(s 
                    => s.Title!.ToLower().Replace(" ", "") == title.ToLower())
                .ToListAsync(cancellationToken);
        }

        public static Task<List<Schedule>> FindByTitleGAsync(this DbSet<Schedule> schedules, string title, CancellationToken cancellationToken = default)
        {
            title = title.Replace(" ", "");
            return schedules
                .Include(s => s.Channel)
                .Where(s
                    => s.Title!.ToLower().Replace(" ", "") == title.Replace('G', '&').ToLower())
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Removes all whitespace from a string (\n, \r, \t)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="replacementValue"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpace(this string value, string replacementValue = "")
        {
            return value
                .Replace("\n", replacementValue)
                .Replace("\r", replacementValue)
                .Replace("\t", replacementValue);
        }
    }
}
