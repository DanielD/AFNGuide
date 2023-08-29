namespace AfnGuideAPI.HostedServices
{
    public abstract class ConsecutiveBackgroundServiceBase : BackgroundServiceBase
    {
        protected ConsecutiveBackgroundServiceBase(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory, 
            IMemoryCache memoryCache) 
            : base(serviceScopeFactory, loggerFactory, memoryCache)
        {
        }

        public static bool IsRssCompleted { get; set; } = false;
        public static bool IsPromoCompleted { get; set; } = false;
    }
}
