﻿using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines sort options
    /// </summary>
    public class SortOptions : IInnerClone<SortOptions>
    {
        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public SortOptions Clone()
        {
            return new SortOptions();
        }
    }
}
