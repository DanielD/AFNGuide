using AfnGuideAPI.Data;
using AfnGuideAPI.Data.SearchEngines.SportsSchedules;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        private readonly ILogger<SportsController> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SportsController(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache) 
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<SportsController>();
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ViewModels.SearchResult<ViewModels.SportsSchedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search(
            [FromQuery] string[] q, [FromQuery] int tz, [FromQuery] int[]? ch,
            [FromQuery] DateTime? sdt, [FromQuery] DateTime? edt,
            [FromQuery] int? cat, [FromQuery] int? l, [FromQuery] string? ph,
            [FromQuery] string[]? uw, [FromQuery] int? sz = 10, [FromQuery] int? p = 1)
        {
            if (tz < 1)
            {
                return BadRequest("TimeZoneId (tz) is required.");
            }
            var timeZone = await GetTimeZoneAsync(tz);
            if (timeZone == null)
            {
                return NotFound("TimeZoneId (tz) not found.");
            }

            try
            {
                p ??= 1;
                sz ??= 10;
                ch ??= (await GetSportsChannelsAsync())!.Select(c => c.Id).ToArray();
                ph ??= string.Empty;
                uw ??= Array.Empty<string>();
                q ??= Array.Empty<string>();
                bool isLive = l == 1;

                uw = uw.Select(w => w.Replace("\"", string.Empty).Trim()).ToArray();

                var searchQuery = new SearchQuery
                {
                    Channels = ch,
                    EndDate = edt,
                    IsLive = isLive,
                    SearchPhrase = ph,
                    SearchWords = q,
                    StartDate = sdt,
                    UnwantedWords = uw,
                    SportCategoryId = cat
                };

                var cacheKey = $"SportsScheduleController.Search({JsonConvert.SerializeObject(searchQuery)})";
                var result = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);

                    using var searchEngine = new Data.SearchEngines.SportsSchedules.SearchEngine(_loggerFactory, _serviceScopeFactory);
                    var result = await searchEngine.SearchAsync(searchQuery);

                    return result;
                });

                var totalCount = result!.TotalCount;
                if (totalCount == 0)
                {
                    return Ok(new ViewModels.SearchResult<ViewModels.SportsSchedule>
                    {
                        Total = 0,
                        Page = 1,
                        Schedules = new List<ViewModels.SportsSchedule>()
                    });
                }

                if (p > 1 && p > Math.Ceiling(Convert.ToDecimal(totalCount / sz)))
                {
                    return BadRequest($"Page number ({p}) is out of range.");
                }

                var output = new ViewModels.SearchResult<ViewModels.SportsSchedule>
                {
                    Total = totalCount,
                    Page = p.Value,
                    Schedules = result.Schedules
                        .Skip((p!.Value - 1) * sz!.Value)
                        .Take(sz!.Value)
                        .Select(s => new ViewModels.SportsSchedule
                        {
                            Id = s.AfnId,
                            ChannelId = s.ChannelId,
                            AirDateLocal = s.AirDateUTC.AddMinutes(timeZone.GetCurrentOffset()),
                            IsLive = s.IsLive,
                            SportsCategoryId = s.SportsCategoryId,
                            IsTapeDelayed = s.IsTapeDelayed,
                            SportName = s.SportName,
                        })
                    .ToList()
                };

                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SportsController.Search");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("categories")]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.SportsCategory>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var sportsCategories = await _cache.GetOrCreateAsync(CacheKeys.SportsCategories, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(4);

                    var dbSportsCategories = await _dbContext.SportsCategories
                        .Where(sc => sc.IsActive)
                        .OrderBy(sc => sc.Name)
                        .ToListAsync();

                    var sportsCategoryViewModels = dbSportsCategories.Select(sc 
                        => new ViewModels.SportsCategory(sc)).ToList();

                    return sportsCategoryViewModels;
                });

                return Ok(sportsCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sports categories.");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
