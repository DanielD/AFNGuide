using AfnGuideAPI.Models;

namespace AfnGuideAPI.Data.SearchEngines.Schedules
{
    public class SearchEngine : SearchEngineBase<Schedule>
    {
        public SearchEngine(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
            : base(loggerFactory, serviceScopeFactory)
        {
        }
        
        protected override async Task<ISearchResults<Schedule>> SearchInternalAsync(
            ISearchQuery query)
        {
            var q = query as SearchQuery;
            var result = await _dbContext.ScheduleSearchAsync(
                q.SearchWords, q.Channels, q.StartDate, q.EndDate,
                q.Rating?.Split(','), q.SearchField?.Trim(), q.SearchPhrase?.Trim(),
                q.UnwantedWords);

            result.Query = q;

            return result;
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
