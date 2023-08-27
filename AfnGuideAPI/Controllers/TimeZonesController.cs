using Microsoft.AspNetCore.Mvc;
using TimeZone = AfnGuideAPI.Models.TimeZone;
using TimeZoneUI = AfnGuideAPI.ViewModels.TimeZone;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZonesController : ControllerBase
    {
        private readonly ILogger<TimeZonesController> _logger;

        public TimeZonesController(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache) 
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = _loggerFactory.CreateLogger<TimeZonesController>();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TimeZoneUI>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var dbTimeZones = await GetTimeZonesAsync();
                var timeZones = from tz in dbTimeZones!
                                select new TimeZoneUI
                                {
                                    Id = tz.Id,
                                    Name = tz.ToString(),
                                    Abbreviation = tz.Abbreviation
                                };
                return Ok(timeZones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time zones");
                return StatusCode(500);
            }
        }
    }
}
