using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;

namespace AfnGuideAPI.HostedServices
{
    public partial class BulletinIngestionService : BackgroundServiceBase
    {
        private readonly ILogger<BulletinIngestionService> _logger;

        public BulletinIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<BulletinIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            // Repeat every 24 hours
            while (!stoppingToken.IsCancellationRequested)
            {
                await IngestBulletins(stoppingToken);

                // Wait 24 hours before starting again.
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task IngestBulletins(CancellationToken stoppingToken)
        {
            // Download text file from website
            using var client = GetNewHttpClient();
            var response = await client
                .GetAsync($"https://myafn.dodmedia.osd.mil/json/bulletins.json?_={DateTime.UtcNow.Ticks}", stoppingToken);
            if (response.IsSuccessStatusCode == false)
                throw new Exception($"Error downloading bulletins. Status code: {response.StatusCode}.");

            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            html = CleanHtml(html);
            // Disable all bulletins in the database
            await DisableOldBulletins();
            // Parse text file into JSON
            var bulletins = JsonConvert.DeserializeObject<List<Bulletin>>(html);
            // Loop through JSON into bulletins
            int order = 1;
            foreach (var bulletin in bulletins!)
            {
                bulletin.Order = order++;
                // Add bulletins to database
                await SaveBulletinToDatabase(bulletin);
            }
        }

        private async Task SaveBulletinToDatabase(Bulletin bulletin)
        {
            try
            {
                // Check if bulletin already exists in database
                var existingBulletin = await _dbContext.Bulletins
                    .FirstOrDefaultAsync(x => x.Id == bulletin.Id);
                // If bulletin already exists, update it
                if (existingBulletin != null)
                {
                    existingBulletin.IsActive = true;
                    existingBulletin.Title = bulletin.Title;
                    existingBulletin.CreatedOnUTC = DateTime.UtcNow;
                    existingBulletin.Order = bulletin.Order;
                }
                // If bulletin doesn't exist, add it
                else
                {
                    bulletin.IsActive = true;
                    bulletin.CreatedOnUTC = DateTime.UtcNow;
                    await _dbContext.Bulletins.AddAsync(bulletin);
                }
                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving bulletin to database.");
            }
        }

        private async Task DisableOldBulletins()
        {
            try
            {
                // Set promos IsActive to false
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE Bulletins SET IsActive = 0 WHERE IsActive = 1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling old bulletins.");
            }
        }

        private static string CleanHtml(string html)
        {
            html = html.Replace("$afn.ProcessBulletinsData([", "[");
            html = html.Replace("])", "]");
            html = html.Replace(@"\u0026quot;", "'");
            html = html.Replace(@"\u0026nbsp;", " ");
            html = html.Replace(@"\u003c", "<");
            html = html.Replace(@"\u003e", ">");
            html = RemoveStyles().Replace(html, "");
            html = RemoveFontTag().Replace(html, "");
            html = html.Replace("</font>", "");

            return html;
        }

        [GeneratedRegex("style=\\\\\"[\\w\\s:\\-:;,\\.']*\\\\\"")]
        private static partial Regex RemoveStyles();
        [GeneratedRegex("\\<font(\\s|)[a-z]*\\b([^>]*?)\\s?\\/?\\>")]
        private static partial Regex RemoveFontTag();
    }
}
