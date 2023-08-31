using AfnGuideAPI.Data;
using AfnGuideAPI.Models;
using AfnGuideAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromosController : ControllerBase
    {
        private readonly ILogger<PromosController> _logger;

        public PromosController(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache) 
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<PromosController>();
        }

        [HttpGet("image/{id}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPromoImage(string id)
        {
            try
            {
                var promo = await _dbContext.Promos.FindAsync(id);
                if (promo == null || promo.ImageData == null || promo.ImageData.Length == 0)
                {
                    return NotFound();
                }

                return File(promo.ImageData, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promo image");
                return StatusCode(500);
            }
        }

        [HttpGet("{timeZoneId:int}")]
        [ProducesResponseType(typeof(List<ViewModels.Promo>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPromos(int timeZoneId)
        {
            try
            {
                var timeZone = await GetTimeZoneAsync(timeZoneId);
                if (timeZone == null)
                {
                    return NotFound();
                }

                if (!_cache.TryGetValue(CacheKeys.Promos, out Dictionary<int, List<ViewModels.Promo>>? cachedPromoLinks))
                {
                    cachedPromoLinks = new();
                }

                if (!cachedPromoLinks!.TryGetValue(timeZoneId, out List<ViewModels.Promo>? promoLinks))
                {
                    var promos = await _dbContext.Promos.ToListAsync();
                    var afnIds = promos.Select(p => p.AfnId).ToList();
                    var schedulesForPromo = (await _dbContext.Schedules
                        .Include(s => s.Channel!)
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
                                : new List<Models.Schedule>();

                            promoLinks.Add(new ViewModels.Promo
                            {
                                Id = promo.AfnId,
                                Title = schedule?.Title ?? promo.Title,
                                Description = schedule?.Description,
                                IsPromoB = promo.IsPromoB,
                                ImageUrl = $"/api/promos/image/{promo.Id}",
                                LinkUrl = promo.Url,
                                Schedules = new PromoSchedules(schedules.Select(s
                                    => new PromoSchedule
                                    {
                                        Channel = schedule?.Channel!.Title,
                                        Time = $"{s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()):t}",
                                        Date = $"{s.AirDateUTC!.AddMinutes(timeZone.GetCurrentOffset()):d}",
                                        TimeZoneId = timeZone.Id
                                    }).ToList())
                            });
                        }

                        cachedPromoLinks.Add(timeZoneId, promoLinks);
                        _cache.Set(CacheKeys.Promos, cachedPromoLinks, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4)
                        });
                    }
                }

                return Ok(promoLinks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving promos");
                return StatusCode(500);
            }
        }
    }
}
