using AfnGuideAPI.Data;

namespace AfnGuideAPI.HostedServices
{
    public abstract class BackgroundServiceBase : BackgroundService
    {
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly AfnGuideDbContext _dbContext;
        protected readonly IMemoryCache _cache;

        protected BackgroundServiceBase(
            IServiceScopeFactory serviceScopeFactory, 
            ILoggerFactory loggerFactory,
            IMemoryCache memoryCache)
        {
            _loggerFactory = loggerFactory;
            _dbContext = serviceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<AfnGuideDbContext>();
            _cache = memoryCache;
        }

        protected static HttpClient GetNewHttpClient(bool emptyUserAgent = false)
        {
            if (emptyUserAgent)
            {
                return new HttpClient();
            }

            var client = new HttpClient();
            client.AddBrowserHeader();
            return client;
        }
    }
}
