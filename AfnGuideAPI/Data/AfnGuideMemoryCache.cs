namespace AfnGuideAPI.Data
{
    public class AfnGuideMemoryCache : IMemoryCache, IDisposable
    {
        public AfnGuideMemoryCache(ILoggerFactory loggerFactory)
        {
            Cache = new MemoryCache(
                new MemoryCacheOptions(),
                loggerFactory);
        }

        public MemoryCache Cache { get; }

        public ICacheEntry CreateEntry(object key)
            => Cache.CreateEntry(key);

        public void Dispose()
            => GC.SuppressFinalize(this);

        public void Remove(object key)
            => Cache.Remove(key);

        public bool TryGetValue(object key, out object? value)
            => Cache.TryGetValue(key, out value);
    }
}
