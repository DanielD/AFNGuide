using AfnGuideAPI.Models;
using Newtonsoft.Json;

namespace AfnGuideAPI.HostedServices
{
    public class ChannelTimeZonesIngestionService : BackgroundServiceBase
    {
        private readonly ILogger<ChannelTimeZonesIngestionService> _logger;

        public ChannelTimeZonesIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<ChannelTimeZonesIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            // Repeat every 24 hours
            while (!stoppingToken.IsCancellationRequested)
            {
                await IngestChannelTimeZones(stoppingToken);

                // Wait 24 hours before starting again.
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task IngestChannelTimeZones(CancellationToken stoppingToken)
        {
            var channels = await _dbContext.Channels
                .Include(x => x.ChannelTimeZones)
                .Where(c => c.IsSplit)
                .ToListAsync(stoppingToken);

            var timeZones = await _dbContext.TimeZones
                .Where(tz => tz.IsActive)
                .ToListAsync(stoppingToken);

            foreach (var channel in channels)
            {
                foreach (var timeZone in timeZones)
                {
                    var channelTimeZone = channel.ChannelTimeZones
                        .FirstOrDefault(ctz => ctz.TimeZoneId == timeZone.Id);

                    if (channelTimeZone == null)
                    {
                        await _dbContext.ChannelTimeZones.AddAsync(new ChannelTimeZone
                        {
                            ChannelId = channel.Id,
                            TimeZoneId = timeZone.Id,
                            CreatedOnUTC = DateTime.UtcNow,
                        }, stoppingToken);
                    }
                }
            }

            await _dbContext.SaveChangesAsync(stoppingToken);

            _logger.LogInformation("Created channel time zones, if needed.");

            var channelTimeZones = await _dbContext.ChannelTimeZones
                .Include(ctz => ctz.Channel)
                .Include(ctz => ctz.TimeZone)
                .ToListAsync(stoppingToken);

            foreach (var channelTimeZone in channelTimeZones)
            {
                // Get the current JSON from the website
                using var client = GetNewHttpClient();
                var response = await client
                    .GetAsync($"https://v3.myafn.dodmedia.osd.mil/api/json/{channelTimeZone.TimeZoneId}/channels.json?_{DateTime.UtcNow.Ticks}", stoppingToken);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON into a list of channels
                    var json = await response.Content.ReadAsStringAsync(stoppingToken);
                    var jsonChannels = JsonConvert.DeserializeObject<List<JsonChannel>>(json);
                    // Find the channel in the list
                    var jsonChannel = jsonChannels?.FirstOrDefault(c 
                        => c.Id == channelTimeZone.ChannelId);
                    if (jsonChannel != null)
                    {
                        // Update the channel's time zone
                        channelTimeZone.StartTime = jsonChannel.StartTime;
                        channelTimeZone.EndTime = jsonChannel.EndTime;
                        channelTimeZone.ModifiedOnUTC = DateTime.UtcNow;

                        _logger.LogInformation($"Updated channel time zone for {channelTimeZone.Channel.Title} in {channelTimeZone.TimeZone.Name}.");
                        await _dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
        }
    }
}
