// "Company © 2025. All rights reserved."

using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines sort options
    /// </summary>
    public class SixnetSortOptions : ISixnetCloneable<SixnetSortOptions>
    {
        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public SixnetSortOptions Clone()
        {
            return new SixnetSortOptions();
        }
    }
}
