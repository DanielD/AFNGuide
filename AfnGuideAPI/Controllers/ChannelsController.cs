using AfnGuideAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> _logger;

        public ChannelsController(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache) 
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<ChannelsController>();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ViewModels.Channel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannels()
        {
            try
            {
                var channels = await _cache.GetOrCreateAsync(CacheKeys.Channels, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(365);
                    return await _dbContext.Channels
                        .OrderBy(c => c.ChannelNumber)
                        .Select(c => new ViewModels.Channel
                        {
                            Id = c.Id,
                            Abbreviation = c.Abbreviation,
                            ChannelNumber = c.ChannelNumber,
                            Color = c.Color,
                            EndTime = c.EndTime,
                            Image = c.Image,
                            IsSplit = c.IsSplit,
                            StartTime = c.StartTime,
                            Title = c.Title
                        })
                        .ToListAsync();
                });

                return Ok(channels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving channels");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
