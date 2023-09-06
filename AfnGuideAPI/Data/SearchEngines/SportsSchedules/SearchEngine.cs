using AfnGuideAPI.Models;

namespace AfnGuideAPI.Data.SearchEngines.SportsSchedules
{
    public class SearchEngine : SearchEngineBase<SportsSchedule>
    {
        public SearchEngine(
            ILoggerFactory loggerFactory, 
            IServiceScopeFactory serviceScopeFactory) 
            : base(loggerFactory, serviceScopeFactory)
        {
        }

        protected override async Task<ISearchResults<SportsSchedule>> SearchInternalAsync(
            ISearchQuery query)
        {
            var q = query as SearchQuery;
            var result = await _dbContext.SportsScheduleSearchAsync(
                q.SearchWords, q.Channels, q.StartDate, q.EndDate,
                q.SportCategoryId, q.SearchPhrase?.Trim(), q.UnwantedWords,
                q.IsLive);

            result.Query = q;

            return result;
        }
    }
}
