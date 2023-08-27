using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
using AfnGuideAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache)
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<ScheduleController>();
        }

        [HttpGet("promoLinks/{timeZoneId:int}")]
        [ProducesResponseType(typeof(List<PromoLink>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPromoLink(int timeZoneId)
        {
            try
            {
                var timeZone = await GetTimeZoneAsync(timeZoneId);
                if (timeZone == null)
                {
                    return NotFound();
                }

                if (!_cache.TryGetValue(CacheKeys.PromoLinks, out Dictionary<int, List<PromoLink>>? cachedPromoLinks))
                {
                    cachedPromoLinks = new();
                }

                if (!cachedPromoLinks!.TryGetValue(timeZoneId, out List<PromoLink>? promoLinks))
                {
                    var promos = await _dbContext.Promos.ToListAsync();
                    var afnIds = promos.Select(p => p.AfnId).ToList();
                    var schedulesForPromo = (await _dbContext.Schedules
                        .Include(s => s.Channel)
                        .Where(s 
                            => afnIds.Contains(s.AfnId) 
                            && s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()) >= timeZone.GetCurrentTime())
                        .OrderBy(s => s.AirDateUTC)
                        .ToListAsync())
                        .ToDictionary(s => s.AfnId);

                    if (promos.Any() && schedulesForPromo.Any())
                    {
                        promoLinks = new();

                        foreach (var promo in promos)
                        {
                            var schedule = schedulesForPromo.GetValueOrDefault(promo.AfnId ?? 0);
                            var schedules = schedule != null 
                                ? await _dbContext.Schedules
                                    .Where(s 
                                        => s.Title == schedule.Title 
                                        && s.Description == schedule.Description
                                        && s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()) >= timeZone.GetCurrentTime())
                                    .OrderBy(s => s.AirDateUTC)
                                    .ToListAsync()
                                : new List<Schedule>();

                            promoLinks.Add(new PromoLink
                            {
                                Title = schedule?.Title ?? promo.Title,
                                Description = schedule?.Description,
                                IsPromoB = promo.IsPromoB,
                                ImageUrl = promo.Image,
                                LinkUrl = promo.Url,
                                Schedules = new PromoLinkSchedules(schedules.Select(s 
                                    => new PromoLinkSchedule
                                    {
                                        Channel = schedule?.Channel!.Title,
                                        Time = $"{s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()):t}",
                                        Date = $"{s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()):d}",
                                        TimeZoneId = timeZone.Id
                                    }).ToList())
                            });
                        }

                        cachedPromoLinks.Add(timeZoneId, promoLinks);
                        _cache.Set(CacheKeys.PromoLinks, cachedPromoLinks, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4)
                        });
                    }
                }

                return Ok(promoLinks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promo link");
                return StatusCode(500);
            }
        }
    }
}
