using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// repository manager
    /// </summary>
    internal static class RepositoryManager
    {
        #region IQuery

        /// <summary>
        /// handle IQuery before execute
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="usageScene">usage scene</param>
        /// <param name="queryHandler">query handler</param>
        /// <returns>real IQuery object to use</returns>
        internal static IQuery HandleQueryObjectBeforeExecute(IQuery query, QueryUsageScene usageScene, Func<IQuery, IQuery> queryHandler = null)
        {
            var newQuery = query?.DeepCopy();
            if (queryHandler != null)
            {
                newQuery = queryHandler(newQuery);
            }
            return newQuery;
        }

        /// <summary>
        /// handle IQuery after execute
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <param name="executeQuery">execute query</param>
        /// <param name="usageScene">usage scene</param>
        internal static void HandleQueryObjectAfterExecute(IQuery originalQuery, IQuery executeQuery, QueryUsageScene usageScene)
        {
            originalQuery?.Reset();
        }

        #endregion
    }
}
