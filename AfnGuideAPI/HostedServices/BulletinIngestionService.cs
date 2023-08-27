namespace AfnGuideAPI.HostedServices
{
    public class BulletinIngestionService : BackgroundServiceBase
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

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Repeat every 24 hours
            while (!stoppingToken.IsCancellationRequested)
            {
                //

                // Wait 24 hours before starting again.
                Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
