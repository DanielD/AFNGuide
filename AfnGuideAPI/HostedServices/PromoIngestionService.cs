using AfnGuideAPI.Models;
using System.Text.RegularExpressions;

namespace AfnGuideAPI.HostedServices
{
    public partial class PromoIngestionService : BackgroundServiceBase
    {
        private readonly ILogger<PromoIngestionService> _logger;

        public PromoIngestionService(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<PromoIngestionService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(PromoIngestionService)} is starting.");

            // Delay 2 minutes to allow the database to be created if necessary
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            // Repeat every 1 hour
            while (!stoppingToken.IsCancellationRequested)
            {
                // Delete old promo files
                await DeleteOldPromos();

#if DEBUG
                await IngestPromos(stoppingToken);
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
#else
                if (_dbContext.Promos.Any())
                {
                    // Promos exist, wait 1 hours before starting.
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    await IngestPromos(stoppingToken);
                }
                else
                {
                    // Promos don't exist, ingest now.
                    await IngestPromos(stoppingToken);
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
#endif
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PromoIngestionService)} is stopping.");

            await base.StopAsync(cancellationToken);
        }

        private async Task IngestPromos(CancellationToken stoppingToken)
        {
            // Download promotions from AFN
            using var client = GetNewHttpClient();
            var response = await client
                .GetAsync("https://myafn.dodmedia.osd.mil/default.aspx", stoppingToken);
            var html = await response.Content.ReadAsStringAsync(stoppingToken);
            html = html.RemoveWhiteSpace();
            // Disable old promos
            await DisableOldPromos();
            // Parse Promo A
            var promoHtml = html[(html.IndexOf("<div id=\"promoASlides\">") + 23)..];
            promoHtml = promoHtml[..promoHtml.IndexOf("<div id=\"promoAControls\">")];
            promoHtml = promoHtml[..promoHtml.LastIndexOf("</div>")];
            var promoMatches = PromoAPattern().Matches(promoHtml);
            foreach (Match m in promoMatches.Cast<Match>())
            {
                var promo = new Promo
                {
                    Id = ParseIdFromImage(m.Groups["image"].Value),
                    AfnId = await AttemptToDetermineAfnId(m.Groups["url"].Value),
                    Duration = int.Parse(m.Groups["duration"].Value),
                    Image = m.Groups["image"].Value,
                    Url = m.Groups.ContainsKey("url") ? m.Groups["url"].Value : null,
                    CreatedOnUTC = DateTime.UtcNow
                };

                // Save Promo A to database
                await SavePromoToDatabase(promo);
            }
            // Parse Promo B
            promoHtml = html[(html.IndexOf("<div class=\"promoB\">") + 20)..];
            promoHtml = promoHtml[..promoHtml.IndexOf("<script type=\"text/javascript\">")];
            promoHtml = promoHtml[..promoHtml.LastIndexOf("</div>")];
            promoMatches = PromoBPattern().Matches(promoHtml);
            // Save Promo B to database
            foreach (Match m in promoMatches.Cast<Match>())
            {
                var promo = new Promo
                {
                    Id = ParseIdFromImage(m.Groups["image"].Value),
                    AfnId = await AttemptToDetermineAfnId(m.Groups["url"].Value),
                    Image = m.Groups["image"].Value,
                    Url = m.Groups.ContainsKey("url") ? m.Groups["url"].Value : null,
                    IsPromoB = true,
                    CreatedOnUTC = DateTime.UtcNow
                };

                // Save Promo A to database
                await SavePromoToDatabase(promo);
            }
        }

        private string ParseIdFromImage(string value)
        {
            var m = PromoIdPattern().Match(value);
            return m.Success ? m.Groups["Id"].Value : Guid.NewGuid().ToString();
        }

        private async Task<int?> AttemptToDetermineAfnId(string value)
        {
            var anchor = value.Contains('#') ? value[(value.IndexOf('#') + 1)..] : null;
            if (anchor != null)
            {
                // Check if anchor matches a valid Schedule title
                var schedule = await _dbContext.Schedules
                    .Where(s 
                        => s.Title!.ToLower().Replace(" ", "") == anchor.ToLower()
                        && s.AirDateUTC >= DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                return schedule?.AfnId;
            }
            return null;
        }

        private async Task SavePromoToDatabase(Promo promo)
        {
            try
            {
                // Check if existing promo exists
                var existingPromo = await _dbContext.Promos
                    .Where(x => x.Id == promo.Id)
                    .FirstOrDefaultAsync();
                // If promo exists, update it
                if (existingPromo != null)
                {
                    existingPromo.AfnId ??= promo.AfnId;
                    existingPromo.Duration = promo.Duration;
                    existingPromo.Image = promo.Image;
                    existingPromo.Url = promo.Url;
                    existingPromo.CreatedOnUTC = promo.CreatedOnUTC;
                    existingPromo.IsPromoB = promo.IsPromoB;
                    existingPromo.IsActive = promo.IsActive;
                }
                else
                {
                    // Otherwise, add it
                    await _dbContext.Promos.AddAsync(promo);
                }
                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving promo to database.");
            }
        }

        private async Task DeleteOldPromos()
        {
            try
            {
                // Delete promos older than 24 hours
                _dbContext.Promos.RemoveRange(_dbContext.Promos.Where(x 
                    => x.CreatedOnUTC < DateTime.UtcNow.AddDays(-1)));
                // Save changes to database
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting old promos.");
            }
        }

        private async Task DisableOldPromos()
        {
            try
            {
                // Set promos IsActive to false
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE Promos SET IsActive = 0 WHERE IsActive = 1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling old promo.");
            }
        }

        [GeneratedRegex("<div\\sclass=\"promoAContainer\"\\sduration=\"(?<duration>[^\"]+)\"\\sstyle=\"background-image:url\\((?<image>[^)]+)\\);\">(<div\\stitle=\"([\\w\\s:/.\\-#]*)\"\\sclass=\"promoALink\"\\surl=\"(?<url>[^\"]+)\"\\sstyle=\"([\\w\\s:;]*)\"><\\/div>|)<\\/div>")]
        private static partial Regex PromoAPattern();

        [GeneratedRegex("(https://imp\\.myafn\\.dodmedia\\.osd\\.mil/web/images/promoaimage\\.ashx\\?id=(?<Id>[^&]+)&|https://myafn\\.dodmedia\\.osd\\.mil/media/cms/promos/(?<Id>[^\\.]+).\\w{3})")]
        private static partial Regex PromoIdPattern();

        [GeneratedRegex("<div\\sclass=\"promoBContainer\"\\sstyle=\"background-image:url\\((?<image>[^)]+)\\);\">(<div\\stitle=\"([\\w\\s:/.\\-#]*)\"\\sclass=\"promoBLink\"\\surl=\"(?<url>[^\"]+)\"\\sstyle=\"([\\w\\s:;]*)\"><\\/div>|)")]
        private static partial Regex PromoBPattern();
    }
}
