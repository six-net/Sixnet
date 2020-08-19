using System;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Repository manager
    /// </summary>
    internal static class RepositoryManager
    {
        /// <summary>
        /// Handle IQuery before execute
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="usageScene">Usage scene</param>
        /// <param name="queryHandler">Query handler</param>
        /// <returns>Return the real query object to use</returns>
        internal static IQuery HandleQueryObjectBeforeExecute(IQuery query, QueryUsageScene usageScene, Func<IQuery, IQuery> queryHandler = null)
        {
            var newQuery = query?.Clone();
            if (queryHandler != null)
            {
                newQuery = queryHandler(newQuery);
            }
            return newQuery;
        }

        /// <summary>
        /// Handle IQuery after execute
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="executeQuery">Execute query</param>
        /// <param name="usageScene">Usage scene</param>
        internal static void HandleQueryObjectAfterExecute(IQuery originalQuery, IQuery executeQuery, QueryUsageScene usageScene)
        {
            originalQuery?.Reset();
        }
    }
}
