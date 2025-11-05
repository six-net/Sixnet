// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory
{
    internal class CacheEntryStack
    {
        private readonly CacheEntryStack _previous;
        private readonly MemoryCacheEntry _entry;

        private CacheEntryStack()
        {
        }

        private CacheEntryStack(CacheEntryStack previous, MemoryCacheEntry entry)
        {
            if (previous == null)
            {
                throw new ArgumentNullException(nameof(previous));
            }

            _previous = previous;
            _entry = entry;
        }

        public static CacheEntryStack Empty { get; } = new CacheEntryStack();

        public CacheEntryStack Push(MemoryCacheEntry c)
        {
            return new CacheEntryStack(this, c);
        }

        public MemoryCacheEntry Peek()
        {
            return _entry;
        }
    }
}
