namespace Sixnet.Cache.Keys.Response
{
    /// <summary>
    /// Delete key response
    /// </summary>
    public class DeleteResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the delete count
        /// </summary>
        public long DeleteCount { get; set; }
    }
}
