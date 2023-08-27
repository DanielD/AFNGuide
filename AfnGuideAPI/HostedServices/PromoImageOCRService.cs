using AfnGuideAPI.Models;
using AfnGuideAPI.QueueService;
using AfnGuideAPI.Services;

namespace AfnGuideAPI.HostedServices
{
    public class PromoImageOCRService : BackgroundServiceBase
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<PromoImageOCRService> _logger;
        private readonly OCRSpaceAPIService _ocrService;

        public PromoImageOCRService(
            IBackgroundTaskQueue taskQueue, 
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache,
            OCRSpaceAPIService ocrService) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _taskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<PromoImageOCRService>();
            _ocrService = ocrService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(PromoImageOCRService)} is starting.");

            // Delay 2 minutes to allow the database to be created if necessary
            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                // Get all promos where IsActive = true and AfnId is null
                var promos = await _dbContext.Promos
                    .Where(p => p.IsActive && p.AfnId == null && p.Image != null)
                    .ToListAsync(cancellationToken: stoppingToken);
                // For each promo, add a task to the queue
                foreach (var promo in promos)
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                    {
                        await ProcessPromoAsync(promo, token);
                    });
                }
                // Delay for 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(PromoImageOCRService)} is stopping.");

            await base.StopAsync(cancellationToken);
        }

        private async Task ProcessPromoAsync(Promo promo, CancellationToken stoppingToken)
        {
            try
            {
                var results = await _ocrService.GetTextFromImageAsync(promo.Image!);
                if (results == null || results.Count == 0)
                {
                    _logger.LogWarning($"No results returned for promo {promo.Id}");
                    return;
                }

                var result = results.First();
                var lines = result.ParsedText!.Split(Environment.NewLine);
                string query = string.Empty;
                string title = string.Empty;
                foreach (var line in lines)
                {
                    bool hasG = false;
                    title += line.Trim() + " ";
                    query += line;
                    query = query.Trim().RemoveWhiteSpace();
                    // Check schedules for a match
                    var schedules = await _dbContext.Schedules.FindByTitleAsync(query, stoppingToken);
                    if (!schedules.Any())
                    {
                        schedules = await _dbContext.Schedules.FindByTitleGAsync(query, stoppingToken);
                        hasG = true;
                    }
                    if (schedules.Any())
                    {
                        promo.AfnId = schedules.First().AfnId;
                        if (hasG)
                        {
                            promo.Title = title.Replace("G", " & ").Trim().RemoveWhiteSpace(" "); //.ToTitleCase();
                        }
                        else
                        {
                            promo.Title = title.Trim().RemoveWhiteSpace(" "); //.ToTitleCase();
                        }
                        await _dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation($"Promo {promo.Id} matched to schedule {schedules.First().Id}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing promo {promo.Id}");
            }
        }
    }
}
