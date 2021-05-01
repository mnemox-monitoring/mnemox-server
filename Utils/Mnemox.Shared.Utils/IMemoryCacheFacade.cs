using System;

namespace Mnemox.Shared.Utils
{
    public interface IMemoryCacheFacade
    {
        TItem Get<TItem>(object key);
        void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow);
    }
}