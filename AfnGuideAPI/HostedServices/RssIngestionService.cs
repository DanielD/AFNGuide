using AfnGuideAPI.Models;
using System.Xml.Linq;

namespace AfnGuideAPI.HostedServices
{
    public class RssIngestionService : ConsecutiveBackgroundServiceBase
    {
        private readonly ILogger<RssIngestionService> _logger;

        private readonly Dictionary<int, string> _channels = new()
        {
            {1, "https://media.myafn.dodmedia.osd.mil/rss/Sports.xml" },
            {2, "https://media.myafn.dodmedia.osd.mil/rss/Prime-Atlantic.xml" },
            {3, "https://media.myafn.dodmedia.osd.mil/rss/Spectrum.xml" },
            {4, "https://media.myafn.dodmedia.osd.mil/rss/Prime-Pacific.xml" },
            {5, "https://media.myafn.dodmedia.osd.mil/rss/News.xml" },
            {6, "https://media.myafn.dodmedia.osd.mil/rss/Xtra.xml" },
            {8, "https://media.myafn.dodmedia.osd.mil/rss/Family.xml" },
            {9, "https://media.myafn.dodmedia.osd.mil/rss/Movie.xml" }
        };

        public RssIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<RssIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Repeat every 4 hours
            while (!stoppingToken.IsCancellationRequested)
            {
                IsRssCompleted = false;

                // Delete old schedule files
                await DeleteOldScheduleFiles();

#if DEBUG
                await IngestRssFeed(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
#else
                if (_dbContext.Schedules.Any())
                {
                    // Schedules exist, wait 4 hours before starting.
                    await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
                    await IngestRssFeed(stoppingToken);
                }
                else
                {
                    // Schedules don't exist, ingest now.
                    await IngestRssFeed(stoppingToken);
                    await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
                }
#endif
            }
        }

        private async Task IngestRssFeed(CancellationToken stoppingToken)
        {
            using var client = GetNewHttpClient();
            // Loop through each channel
            foreach (int channelId in _channels.Keys)
            {
                var response = await client.GetAsync(_channels[channelId], stoppingToken);
                if (response.IsSuccessStatusCode == false)
                {
                    _logger.LogError($"Error ingesting RSS feed for channel {channelId}");
                    continue;
                }

                var content = await response.Content.ReadAsStringAsync(stoppingToken);
                content = content.Replace("afn:", "afn_");
                var rss = XDocument.Parse(content);
                var items = rss.Descendants("item");
                // Parse RSS feed
                foreach (var item in items)
                {
                    var afnId = item.Attribute("id")!.Value.ToInt32();
                    var title = item.Element("title")?.Value;
                    var description = item.Element("description")?.Value;
                    var category = item.Element("category")?.Value;
                    var airdate = item.Element("afn_airdate")?.Value?.ToDateString().ToDateTime();
                    var episodeTitle = item.Element("afn_episodeTitle")?.Value;
                    var duration = item.Element("afn_duration")?.Value?.ToInt32();
                    var genre = item.Element("afn_genre")?.Value;
                    var rating = item.Element("afn_rating")?.Value?.Trim();
                    var year = item.Element("afn_year")?.Value?.ToInt32();
                    var premiere = item.Element("afn_premiere")?.Value?.ToBoolean();

                    var schedule = new Schedule
                    {
                        Id = Guid.NewGuid(),
                        AfnId = afnId!.Value,
                        AirDateUTC = airdate!.Value,
                        Category = category,
                        Description = description,
                        Duration = duration!.Value,
                        EpisodeTitle = episodeTitle,
                        Genre = genre,
                        IsPremiere = premiere!.Value,
                        Rating = rating,
                        Title = title,
                        Year = year,
                        ChannelId = channelId,
                        CreatedOnUTC = DateTime.UtcNow
                    };

                    // Save RSS feed to database
                    await SaveRssFeedToDatabase(schedule);
                }
            }

            IsRssCompleted = true;
        }

        private async Task DeleteOldScheduleFiles()
        {
            try
            {
                // Delete schedules where air date is older than 30 days
                _dbContext.Schedules.RemoveRange(_dbContext.Schedules.Where(s 
                    => s.AirDateUTC < DateTime.UtcNow.AddDays(-30)));

                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old schedule files");
            }
        }

        private async Task SaveRssFeedToDatabase(Schedule schedule)
        {
            try
            {
                // Check if schedule already exists in database
                var existingSchedule = await _dbContext.Schedules
                    .FirstOrDefaultAsync(s => s.AfnId == schedule.AfnId);
                // If schedule exists, update it
                if (existingSchedule != null)
                {
                    existingSchedule.AirDateUTC = schedule.AirDateUTC;
                    existingSchedule.Category = schedule.Category;
                    existingSchedule.Description = schedule.Description;
                    existingSchedule.Duration = schedule.Duration;
                    existingSchedule.EpisodeTitle = schedule.EpisodeTitle;
                    existingSchedule.Genre = schedule.Genre;
                    existingSchedule.IsPremiere = schedule.IsPremiere;
                    existingSchedule.Rating = schedule.Rating;
                    existingSchedule.Title = schedule.Title;
                    existingSchedule.Year = schedule.Year;
                    existingSchedule.ChannelId = schedule.ChannelId;
                }
                else
                {
                    // If schedule does not exist, add it
                    await _dbContext.Schedules.AddAsync(schedule);
                }
                // Save schedule to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving RSS feed to database");
            }
        }
    }
}
