using System;
using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Queryable filter
    /// </summary>
    [Serializable]
    public abstract class QueryableFilter : ISixnetMappable
    {
        /// <summary>
        /// Generate a IQueryable instance 
        /// </summary>
        /// <param name="useForPaging">Indecates whether use for paging</param>
        /// <returns>Return a IQuery instance</returns>
        public abstract ISixnetQueryable CreateQueryable(bool useForPaging = false);
    }
}
