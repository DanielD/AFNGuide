using AfnGuideAPI.Data;
using Microsoft.AspNetCore.Mvc;
using TimeZone = AfnGuideAPI.Models.TimeZone;

namespace AfnGuideAPI.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly AfnGuideDbContext _dbContext;
        protected readonly IMemoryCache _cache;
        protected readonly ILoggerFactory _loggerFactory;

        protected ControllerBase(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache)
        {
            _loggerFactory = loggerFactory;
            _dbContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<AfnGuideDbContext>();
            _cache = memoryCache;
        }

        protected async Task<List<TimeZone>?> GetTimeZonesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.TimeZones, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                return await _dbContext.TimeZones.ToListAsync();
            });
        }

        protected async Task<TimeZone?> GetTimeZoneAsync(int timeZoneId)
        {
            var timeZones = await GetTimeZonesAsync();
            return timeZones?.FirstOrDefault(tz => tz.Id == timeZoneId);
        }
    }
}
