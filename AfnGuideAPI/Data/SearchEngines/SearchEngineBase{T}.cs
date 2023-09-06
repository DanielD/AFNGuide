namespace AfnGuideAPI.Data.SearchEngines
{
    public abstract class SearchEngineBase<TSchedule>
        : ISearchEngine
        , IDisposable
        where TSchedule : class
    {
        private readonly ILogger<SearchEngineBase<TSchedule>> _logger;
        protected readonly AfnGuideDbContext _dbContext;

        public SearchEngineBase(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = loggerFactory.CreateLogger<SearchEngineBase<TSchedule>>();
            _dbContext = serviceScopeFactory.CreateScope().ServiceProvider
                .GetRequiredService<AfnGuideDbContext>();
        }

        public async Task<ISearchResults<TSchedule>> SearchAsync(ISearchQuery q)
        {
            try
            {
                return await SearchInternalAsync(q);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for schedules.");
                throw;
            }
        }

        protected abstract Task<ISearchResults<TSchedule>> SearchInternalAsync(ISearchQuery q);

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            ((IDisposable)_dbContext).Dispose();
        }
    }

    public interface ISearchEngine { }

    public interface ISearchQuery { }

    public interface ISearchResults<TSchedule> where TSchedule : class
    {
        int TotalCount { get; set; }
        ISearchQuery Query { get; set; }
        List<TSchedule> Schedules { get; set; }
    }
}
