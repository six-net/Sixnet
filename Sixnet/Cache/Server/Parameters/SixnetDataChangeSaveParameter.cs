// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Data change save parameter
    /// </summary>
    public class SixnetDataChangeSaveParameter
    {
        /// <summary>
        /// Gets or sets the time（seconds）
        /// </summary>
        public long Seconds { get; set; }

        /// <summary>
        /// Gets or sets the changes number
        /// </summary>
        public long Changes { get; set; }
    }
}
