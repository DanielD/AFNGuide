using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
using Newtonsoft.Json;

namespace AfnGuideAPI.HostedServices
{
    public class SportsIngestionService : BackgroundServiceBase
    {
        private readonly ILogger<SportsIngestionService> _logger;

        //private List<SportsNetwork>? _sportsNetworks;
        private List<SportsCategory>? _sportsCategories;

        public SportsIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<SportsIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            // Repeat every 4 hours
            int _ = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                // Get sports networks from JSON
                //_ = await IngestSportsNetworks(stoppingToken);
                // Get sports categories from JSON
                _ = await IngestSportsCategories(stoppingToken);
                // Get sports schedules from SearchSports.axd
                await IngestSportsSchedules(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
            }
        }

        private async Task IngestSportsSchedules(CancellationToken stoppingToken)
        {
            // Get channels from database
            var channels = await _dbContext.Channels
                .Where(x => x.IsSports)
                .Select(x => x.Id)
                .ToListAsync(stoppingToken);
            // Download sports data from website using POST
            using var client = GetNewHttpClient();
            var formData = new MultipartFormDataContent
            {
                { new StringContent("32"), "sTimeZone" }, // UTC
                { new StringContent(string.Join('|', channels)), "sChannel" },
                { new StringContent("1000"), "sCount" },
                { new StringContent(""), "sDateFrom" },
                { new StringContent(""), "sDateTo" },
                { new StringContent("All"), "sSport" },
                { new StringContent("All"), "sNetwork" },
                { new StringContent("false"), "sLiveOnly" },
                { new StringContent(""), "sWord" },
                { new StringContent(""), "sPhrase" },
                { new StringContent(""), "sUnwanted" },
                { new StringContent("1"), "sPageNo" },
            };
            var response = await client.PostAsync("https://myafn.dodmedia.osd.mil/SearchSports.axd", formData, stoppingToken);
            if (response.IsSuccessStatusCode == false)
                throw new Exception($"Error downloading sports. Status code: {response.StatusCode}.");

            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            // Delete old sports from database older than 1 day
            await DeleteOldSports(stoppingToken);
            // Parse text file into JSON
            var sports = JsonConvert.DeserializeObject<JsonSportsSchedule>(html);
            // Loop through JSON into sports
            foreach (var sport in sports!.SportsScheduleItems!)
            {
                // Add sports to database
                await SaveSportScheduleToDatabase(sport);
            }
        }

        private async Task SaveSportScheduleToDatabase(JsonScportsScheduleItem sport)
        {
            // Check if sports schedule already exists in database
            var sportsSchedule = await _dbContext.SportsSchedules
                .FirstOrDefaultAsync(x => x.AfnId == sport.AfnId);
            // If it does, update it
            if (sportsSchedule != null)
            {
                sportsSchedule.SportsCategoryId = determineSportCategoryId(sport.SportCategory);
                sportsSchedule.SportName = sport.SportName;
                sportsSchedule.AirDateUTC = sport.AirDateUTC;
                sportsSchedule.IsLive = sport.IsLive;
                sportsSchedule.IsTapeDelayed = sport.IsTapeDelayed;
                sportsSchedule.ModifiedOnUTC = DateTime.UtcNow;
            }
            // If it doesn't, add it
            else
            {
                var newSportsSchedule = new SportsSchedule
                {
                    AfnId = sport.AfnId,
                    ChannelId = sport.ChannelId,
                    SportsCategoryId = determineSportCategoryId(sport.SportCategory),
                    SportName = sport.SportName,
                    AirDateUTC = sport.AirDateUTC,
                    IsLive = sport.IsLive,
                    IsTapeDelayed = sport.IsTapeDelayed,
                    CreatedOnUTC = DateTime.UtcNow,
                };
                await _dbContext.SportsSchedules.AddAsync(newSportsSchedule);
            }
            // Save changes to database
            await _dbContext.SaveChangesAsync();

            int? determineSportCategoryId(string? sportName)
            {
                return _sportsCategories!
                    .FirstOrDefault(x => x.Name == sportName)?.Id;
            }
        }

        private async Task<int> IngestSportsCategories(CancellationToken stoppingToken)
        {
            // Download text file from website
            using var client = GetNewHttpClient();
            var response = await client
                .GetAsync($"https://myafn.dodmedia.osd.mil/json/SportsCategories.json?_={DateTime.UtcNow.Ticks}", stoppingToken);
            if (response.IsSuccessStatusCode == false)
                throw new Exception($"Error downloading sports categories. Status code: {response.StatusCode}.");

            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            html = CleanSportsCategoriesHtml(html);
            // Disable all sports networks in the database
            await DisableOldSportsCategories(stoppingToken);
            // Parse text file into JSON
            var sportsCategories = JsonConvert.DeserializeObject<List<string>>(html);
            // Loop through JSON into sports categories
            foreach (var sportsCategory in sportsCategories!)
            {
                // Add sports categories to database
                await SaveSportsCategoryToDatabase(sportsCategory);
            }
            // Populate local cache
            _sportsCategories = await _cache.GetOrCreateAsync(CacheKeys.SportsCategories, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);

                return await _dbContext.SportsCategories
                    .Where(x => x.IsActive)
                    .ToListAsync(stoppingToken);
            });

            return sportsCategories.Count;
        }

        private async Task<int> IngestSportsNetworks(CancellationToken stoppingToken)
        {
            // Download text file from website
            using var client = GetNewHttpClient();
            var response = await client
                .GetAsync($"https://myafn.dodmedia.osd.mil/json/SportsNetworks.json?_={DateTime.UtcNow.Ticks}", stoppingToken);
            if (response.IsSuccessStatusCode == false)
                throw new Exception($"Error downloading sports networks. Status code: {response.StatusCode}.");

            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            html = CleanSportsNetworksHtml(html);
            // Disable all sports networks in the database
            await DisableOldSportsNetworks(stoppingToken);
            // Parse text file into JSON
            var sportsNetworks = JsonConvert.DeserializeObject<List<string>>(html);
            // Loop through JSON into sports networks
            foreach (var sportsNetwork in sportsNetworks!)
            {
                // Add sports networks to database
                await SaveSportsNetworkToDatabase(sportsNetwork);
            }
            // Populate local cache
            //_sportsNetworks = await _cache.GetOrCreateAsync(CacheKeys.SportsNetworks, async entry =>
            //{
            //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);

            //    return await _dbContext.SportsNetworks
            //        .Where(x => x.IsActive)
            //        .ToListAsync(stoppingToken);
            //});

            return sportsNetworks.Count;
        }

        private async Task SaveSportsNetworkToDatabase(string sportsNetwork)
        {
            try
            {
                // Check if sports network already exists in database
                var sportsNetworks = await _dbContext.SportsNetworks
                    .FirstOrDefaultAsync(x => x.Name == sportsNetwork);
                // If it does, update it
                if (sportsNetworks != null)
                {
                    sportsNetworks.ModifiedOnUTC = DateTime.UtcNow;
                    sportsNetworks.IsActive = true;
                }
                // If it doesn't, add it
                else
                {
                    var newSportsNetwork = new SportsNetwork
                    {
                        Name = sportsNetwork,
                        CreatedOnUTC = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _dbContext.SportsNetworks.AddAsync(newSportsNetwork);
                }
                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving sports network to database. Sports network: {sportsNetwork}");
            }
        }

        private async Task SaveSportsCategoryToDatabase(string sportsCategory)
        {
            try
            {
                // Check if sports category already exists in database
                var sportsCategorys = await _dbContext.SportsCategories
                    .FirstOrDefaultAsync(x => x.Name == sportsCategory);
                // If it does, update it
                if (sportsCategorys != null)
                {
                    sportsCategorys.ModifiedOnUTC = DateTime.UtcNow;
                    sportsCategorys.IsActive = true;
                }
                // If it doesn't, add it
                else
                {
                    var newSportsCategory = new SportsCategory
                    {
                        Name = sportsCategory,
                        CreatedOnUTC = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _dbContext.SportsCategories.AddAsync(newSportsCategory);
                }
                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving sports network to database. Sports category: {sportsCategory}");
            }
        }

        private async Task DeleteOldSports(CancellationToken stoppingToken)
        {
            try
            {
                // Delete schedules where air date is older than 1 days
                _dbContext.SportsSchedules.RemoveRange(
                    _dbContext.SportsSchedules.Where(s
                        => s.AirDateUTC < DateTime.UtcNow.AddDays(-1)));

                // Save changes to database
                await _dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old sports schedules.");
            }
        }

        private async Task DisableOldSportsNetworks(CancellationToken stoppingToken)
        {
            try
            {
                // Set sports networks IsActive to false
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE SportsNetworks SET IsActive = 0 WHERE IsActive = 1",
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling old sports networks.");
            }
        }

        private async Task DisableOldSportsCategories(CancellationToken stoppingToken)
        {
            try
            {
                // Set sports networks IsActive to false
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE SportsCategories SET IsActive = 0 WHERE IsActive = 1",
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling old sports categories.");
            }
        }

        private static string CleanSportsNetworksHtml(string html)
        {
            html = html.Replace("$afn.ProcessSportsNetworkData([", "[");
            html = html.Replace("])", "]");
            html = html.Replace(@"\u0026quot;", "'");
            html = html.Replace(@"\u0026nbsp;", " ");
            html = html.Replace(@"\u0026", "&");

            return html;
        }

        private static string CleanSportsCategoriesHtml(string html)
        {
            html = html.Replace("$afn.ProcessSportsCategoryData([", "[");
            html = html.Replace("])", "]");
            html = html.Replace(@"\u0026quot;", "'");
            html = html.Replace(@"\u0026nbsp;", " ");
            html = html.Replace(@"\u0026", "&");

            return html;
        }
    }
}
