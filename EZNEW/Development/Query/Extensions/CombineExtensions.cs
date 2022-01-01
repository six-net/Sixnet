using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    public static class CombineExtensions
    {
        #region UnionAll

        /// <summary>
        /// Add a union all item
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="combineQuery">Combine query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery UnionAll(this IQuery sourceQuery, IQuery combineQuery)
        {
            if (combineQuery == null)
            {
                return sourceQuery;
            }
            return sourceQuery.Combine(new CombineEntry()
            {
                Type = CombineType.UnionAll,
                Query = combineQuery
            });
        }

        #endregion

        #region Union

        /// <summary>
        /// Add a union item
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="combineQuery">Combine query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Union(this IQuery sourceQuery, IQuery combineQuery)
        {
            if (combineQuery == null)
            {
                return sourceQuery;
            }
            return sourceQuery.Combine(new CombineEntry()
            {
                Type = CombineType.Union,
                Query = combineQuery
            });
        }

        #endregion

        #region Except

        /// <summary>
        /// Add a except item
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="combineQuery">Combine query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Except(this IQuery sourceQuery, IQuery combineQuery)
        {
            if (combineQuery == null)
            {
                return sourceQuery;
            }
            return sourceQuery.Combine(new CombineEntry()
            {
                Type = CombineType.Except,
                Query = combineQuery
            });
        }

        #endregion

        #region Intersect

        /// <summary>
        /// Add a intersect item
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="combineQuery">Combine query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Intersect(this IQuery sourceQuery, IQuery combineQuery)
        {
            if (combineQuery == null)
            {
                return sourceQuery;
            }
            return sourceQuery.Combine(new CombineEntry()
            {
                Type = CombineType.Intersect,
                Query = combineQuery
            });
        }

        #endregion
    }
}
