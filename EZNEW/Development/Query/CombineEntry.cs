using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
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
        /// Gets or sets the combine query
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Clone a new combine entry
        /// </summary>
        /// <returns></returns>
        public CombineEntry Clone()
        {
            return new CombineEntry()
            {
                Type = Type,
                Query = Query?.Clone()
            };
        }
    }
}
