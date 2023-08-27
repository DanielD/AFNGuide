using AfnGuideAPI.HostedServices;

namespace AfnGuideAPI.QueueService
{
    public class QueuedHostedService : BackgroundServiceBase
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(
            IBackgroundTaskQueue taskQueue,
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
            _taskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(QueuedHostedService)} is starting.");

            return ProcessTaskQueueAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(QueuedHostedService)} is stopping.");

            await base.StopAsync(cancellationToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred executing {nameof(workItem)}.");
                }
            }
        }
    }
}
