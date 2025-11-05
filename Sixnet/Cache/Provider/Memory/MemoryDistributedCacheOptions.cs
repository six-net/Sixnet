// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Provider.Memory
{
    internal class MemoryDistributedCacheOptions : MemoryCacheOptions
    {
        public MemoryDistributedCacheOptions()
            : base()
        {
            // Default size limit of 200 MB
            SizeLimit = 200 * 1024 * 1024;
        }
    }
}