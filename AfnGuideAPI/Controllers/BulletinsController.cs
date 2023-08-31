using Microsoft.AspNetCore.Mvc;
using AfnGuideAPI.ViewModels;
using AfnGuideAPI.Data;

namespace AfnGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulletinsController : ControllerBase
    {
        private readonly ILogger<BulletinsController> _logger;

        public BulletinsController(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory, 
            IMemoryCache memoryCache) 
            : base(loggerFactory, serviceScopeFactory, memoryCache)
        {
            _logger = loggerFactory.CreateLogger<BulletinsController>();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Bulletin>), 200)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Try to get the bulletins from the cache
                var bulletins = await _cache.GetOrCreateAsync(CacheKeys.Bulletins, async entry =>
                {
                    // If the bulletins are not in the cache, get them from the database
                    var dbBulletins = await _dbContext.Bulletins
                        .Where(b => b.IsActive)
                        .OrderBy(b => b.Order)
                        .ToListAsync();
                    // Convert the bulletins to a list of BulletinViewModels
                    var bulletinViewModels = dbBulletins.Select(b => new Bulletin(b)).ToList();
                    // Set the cache expiration
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                    // Return the list of BulletinViewModels
                    return bulletinViewModels;
                });

                return Ok(bulletins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bulletins");
                return StatusCode(500);
            }
        }
    }
}
