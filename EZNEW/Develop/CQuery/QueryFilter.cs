using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Query filter
    /// </summary>
    [Serializable]
    public abstract class QueryFilter
    {
        /// <summary>
        /// Gets or sets the return data count
        /// Default value is 0,which means return all data
        /// </summary>
        public int QuerySize { get; set; } = 0;

        /// <summary>
        /// Generate a IQuery instance 
        /// </summary>
        /// <returns>Return a IQuery instance</returns>
        public abstract IQuery CreateQuery();
    }
}
