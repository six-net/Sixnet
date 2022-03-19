﻿using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Sort entry
    /// </summary>
    [Serializable]
    public class SortEntry : IInnerClone<SortEntry>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public FieldInfo Field { get; set; }

        /// <summary>
        /// Indicates whether order by desc
        /// </summary>
        public bool Desc { get; set; }

        /// <summary>
        /// Gets or sets the sort options
        /// </summary>
        public SortOptions Options { get; set; }

        /// <summary>
        /// Clone a sort entry
        /// </summary>
        /// <returns></returns>
        public SortEntry Clone()
        {
            return new SortEntry()
            {
                Field = Field?.Clone(),
                Desc = Desc,
                Options = Options?.Clone()
            };
        }
    }
}
