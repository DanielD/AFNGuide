using Microsoft.AspNetCore.Mvc;

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
    }
}
