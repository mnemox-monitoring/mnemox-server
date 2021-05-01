using Microsoft.Extensions.Caching.Memory;
using System;

namespace Mnemox.Shared.Utils
{
    public class MemoryCacheFacade : IMemoryCacheFacade
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheFacade(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
        }

        public TItem Get<TItem>(object key)
        {
            return _memoryCache.Get<TItem>(key);
        }
    }
}
