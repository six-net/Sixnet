// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash get all result
    /// </summary>
    public class SixnetHashGetAllResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues { get; set; }
    }
}
