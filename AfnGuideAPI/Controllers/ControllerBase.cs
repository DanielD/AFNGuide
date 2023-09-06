using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
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

        protected async Task<List<Channel>?> GetChannelsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.Channels, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                return await _dbContext.Channels.ToListAsync();
            });
        }

        protected async Task<List<Channel>?> GetSportsChannelsAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.SportsChannels, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                return await _dbContext.Channels.Where(c => c.IsSports).ToListAsync();
            });
        }

        protected async Task<List<TVSeries>?> GetTVSeriesAsync()
        {
            return await _cache.GetOrCreateAsync(CacheKeys.TVSeries, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
                return await _dbContext.TVSeries.ToListAsync();
            });
        }
    }
}
