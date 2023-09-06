using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
using Newtonsoft.Json;

namespace AfnGuideAPI.HostedServices
{
    public class TVSeriesIngestionService : BackgroundServiceBase
    {
        private readonly ILogger<TVSeriesIngestionService> _logger;
        private List<Channel> _channels = new();

        public TVSeriesIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<TVSeriesIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await IngestTVSeries(stoppingToken);

                // Wait 24 hours before starting again.
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task IngestTVSeries(CancellationToken stoppingToken)
        {
            // Get the channels from the database
            _channels = await _dbContext.Channels.ToListAsync(stoppingToken);

            // Get the data from the website
            using var client = GetNewHttpClient();
            var response = await client
                .GetAsync($"https://myafn.dodmedia.osd.mil/json/Projection.json?_={DateTime.UtcNow.Ticks}", stoppingToken);
            if (response.IsSuccessStatusCode == false)
                throw new Exception($"Error downloading TV Series. Status code: {response.StatusCode}.");

            // Clean the html
            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            html = CleanHtml(html);
            // Delete all old TV Series from the database
            await DeleteOldTVSeries(stoppingToken);
            // Convert the html to JSON
            var tvSeries = JsonConvert.DeserializeObject<List<JsonTVSeries>>(html);
            // Loop through the TV Series and add them to the database
            foreach (var tvSerie in tvSeries!)
            {
                // Add TV Series to the database
                await SaveTVSeriesToDatabase(tvSerie, stoppingToken);
            }
            // Populate the local cache
            await PopulateLocalCache(stoppingToken);
        }

        private async Task PopulateLocalCache(CancellationToken stoppingToken)
        {
            await _cache.GetOrCreateAsync(CacheKeys.TVSeries, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

                return await _dbContext.TVSeries
                    .OrderBy(x => x.Name)
                    .ToListAsync(stoppingToken);
            });
        }

        private async Task SaveTVSeriesToDatabase(JsonTVSeries tvSerie, CancellationToken stoppingToken)
        {
            try
            {
                // Check if the TV Series already exists in the database
                var incomingChannelId = determineChannelId(tvSerie.ChannelName);
                var existingTVSeries = _dbContext.TVSeries
                    .FirstOrDefault(x 
                        => x.Name == tvSerie.Name
                        && x.Season == tvSerie.Season
                        && x.StartDate == tvSerie.StartDate
                        && x.ChannelId == incomingChannelId);
                // If it does, update it
                if (existingTVSeries != null)
                {
                    existingTVSeries.ModifiedOnUTC = DateTime.UtcNow;
                }
                // If it doesn't, add it
                else
                {
                    var newTvSeries = new TVSeries
                    {
                        ChannelId = incomingChannelId,
                        Name = tvSerie.Name,
                        Season = tvSerie.Season,
                        StartDate = tvSerie.StartDate,
                        IsSplit = isSplit(tvSerie.ChannelName),
                        PremiereType = determinPremiereType(tvSerie.PremiereType),
                        CreatedOnUTC = DateTime.UtcNow,
                    };
                    await _dbContext.TVSeries.AddAsync(newTvSeries, stoppingToken);
                }
                // Save the changes to the database
                await _dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving TV Series to database.");
            }

            int? determineChannelId(string? channelName)
            {
                return _channels
                    .FirstOrDefault(x => x.Title?.Contains(channelName!) == true)?.Id;
            }

            int? determinPremiereType(string? premiereType)
            {
                if (premiereType == "Season Premiere")
                    return 1;
                if (premiereType == "Series Premiere")
                    return 2;
                if (premiereType == "Series Finale")
                    return 3;
                if (premiereType == "Season Finale")
                    return 4;
                if (premiereType == "Special")
                    return 5;
                if (premiereType == "Mid-Season Premiere")
                    return 6;

                return null;
            }

            bool isSplit(string? channelName)
            {
                return channelName?.Contains("AFN|pulse") ?? false;
            }
        }

        private async Task DeleteOldTVSeries(CancellationToken stoppingToken)
        {
            try
            {
                // Delete TV Series where StartDate is older than 1 day
                _dbContext.TVSeries.RemoveRange(
                    _dbContext.TVSeries.Where(x 
                        => x.StartDate < DateTime.UtcNow.AddDays(-1)));

                // Save the changes to the database
                await _dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old TV Series.");
            }
        }

        private static string CleanHtml(string html)
        {
            html = html.Replace("$afn.ProcessProjectionData([", "[");
            html = html.Replace("])", "]");
            html = html.Replace(@"\u0026quot;", "'");
            html = html.Replace(@"\u0026nbsp;", " ");
            html = html.Replace(@"\u0027", "'");
            html = html.Replace(@"\u003c", "<");
            html = html.Replace(@"\u003e", ">");

            return html;
        }
    }
}
