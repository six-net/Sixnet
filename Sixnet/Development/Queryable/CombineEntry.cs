using System;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Combine entry
    /// </summary>
    [Serializable]
    public class CombineEntry
    {
        /// <summary>
        /// Gets or sets the combine type
        /// </summary>
        public CombineType Type { get; set; }

        /// <summary>
        /// Gets or sets the target queryable
        /// </summary>
        public ISixnetQueryable TargetQueryable { get; set; }

        /// <summary>
        /// Clone a new combine entry
        /// </summary>
        /// <returns></returns>
        public CombineEntry Clone()
        {
            return new CombineEntry()
            {
                Type = Type,
                TargetQueryable = TargetQueryable?.Clone()
            };
        }
    }
}
