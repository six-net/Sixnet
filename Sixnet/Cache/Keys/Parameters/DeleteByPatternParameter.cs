// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Delete by pattern
    /// </summary>
    public class DeleteByPatternParameter : CacheParameter<CacheResult>
    {
        /// <summary>
        /// Gets the pattern
        /// </summary>
        public string Pattern { get; set; }

        protected override CacheResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            throw new NotImplementedException();
        }

        protected override Task<CacheResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            throw new NotImplementedException();
        }
    }
}
