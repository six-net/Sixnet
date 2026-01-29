// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Delete by pattern
    /// </summary>
    public class SixnetDeleteByPatternParameter : SixnetCacheParameter<SixnetCacheResult>
    {
        /// <summary>
        /// Gets the pattern
        /// </summary>
        public string Pattern { get; set; }

        protected override SixnetCacheResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            throw new NotImplementedException();
        }

        protected override Task<SixnetCacheResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            throw new NotImplementedException();
        }
    }
}
