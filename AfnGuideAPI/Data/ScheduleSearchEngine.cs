using AfnGuideAPI.Models;

namespace AfnGuideAPI.Data
{
    public class ScheduleSearchEngine : IDisposable
    {
        private readonly ILogger<ScheduleSearchEngine> _logger;
        protected readonly AfnGuideDbContext _dbContext;

        public ScheduleSearchEngine(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<ScheduleSearchEngine>();
            _dbContext = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<AfnGuideDbContext>();
        }

        public async Task<ScheduleSearchResults> SearchAsync(ScheduleSearchQuery q)
        {
            var result = await _dbContext.ScheduleSearch(
                q.SearchWords, q.Channels, q.StartDate, q.EndDate, 
                q.Rating?.Split(','), q.SearchField, q.SearchPhrase, 
                q.UnwantedWords);

            result.Query = q;

            return result;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ((IDisposable)_dbContext).Dispose();
        }
    }

    internal static class ScheduleSearchFields
    {
        public const string All = "ALL";
        public const string Title = "TITLE";
        public const string EpisodeTitle = "EPISODE TITLE";
        public const string Description = "DESCRIPTION";
        public const string Genre = "GENRE";
    }

    internal static class ScheduleRatings
    {
        public const string Any = "ANY";
        public const string TVY = "TV-Y";
        public const string TVY7 = "TV-Y7";
        public const string TVG = "TV-G";
        public const string TVPG = "TV-PG";
        public const string TV14 = "TV-14";
        public const string TVMA = "TV-MA";
    }
}
