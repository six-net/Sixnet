using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Combine item
    /// </summary>
    [Serializable]
    public class CombineItem
    {
        /// <summary>
        /// Gets or sets the combine type
        /// </summary>
        public CombineType CombineType { get; set; }

        /// <summary>
        /// Gets or sets the combine query
        /// </summary>
        public IQuery CombineQuery { get; set; }

        /// <summary>
        /// Clone a new combine item
        /// </summary>
        /// <returns></returns>
        public CombineItem Clone()
        {
            return new CombineItem()
            {
                CombineType = CombineType,
                CombineQuery = CombineQuery?.Clone()
            };
        }
    }
}
