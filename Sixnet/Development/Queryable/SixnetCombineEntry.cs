// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Combine entry
    /// </summary>
    [Serializable]
    public class SixnetCombineEntry
    {
        /// <summary>
        /// Gets or sets the combine type
        /// </summary>
        public SixnetCombineType Type { get; set; }

        /// <summary>
        /// Gets or sets the target queryable
        /// </summary>
        public ISixnetQueryable Target { get; set; }

        /// <summary>
        /// Clone a new combine entry
        /// </summary>
        /// <returns></returns>
        public SixnetCombineEntry Clone()
        {
            return new SixnetCombineEntry()
            {
                Type = Type,
                Target = Target?.Clone()
            };
        }
    }
}
