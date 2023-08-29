using AfnGuideAPI.Models;
using AfnGuideAPI.QueueService;
using AfnGuideAPI.Services;

namespace AfnGuideAPI.HostedServices
{
    public class PromoImageOCRService : ConsecutiveBackgroundServiceBase
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

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!IsPromoCompleted)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                // Get all promos where IsActive = true and AfnId is null
                var promos = await _dbContext.Promos
                    .Where(p => p.IsActive && p.Image != null)
                    .ToListAsync(cancellationToken: stoppingToken);
                // For each promo, add a task to the queue
                foreach (var promo in promos)
                {
                    await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                    {
                        await ProcessPromoAsync(promo, token);
                    });
                }

                IsPromoCompleted = false;
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
                    title += line.Trim() + " ";
                    query += line;
                    query = query.Trim().RemoveWhiteSpace();
                    // Check schedules for a match
                    var schedules = await _dbContext.Schedules.FindByTitleAsync(query, stoppingToken);
                    if (!schedules.Any())
                    {
                        schedules = await _dbContext.Schedules.FindByTitleGAsync(query, stoppingToken);
                    }
                    if (schedules.Any())
                    {
                        promo.AfnId = schedules.First().AfnId;
                        promo.Title = schedules.First().Title;
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
