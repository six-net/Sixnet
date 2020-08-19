using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    public static class CrossJoinExtensions
    {
        /// <summary>
        /// Add a cross join
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery CrossJoin(this IQuery sourceQuery, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var query in joinQuerys)
                {
                    sourceQuery = sourceQuery.Join(string.Empty, string.Empty, JoinType.CrossJoin, JoinOperator.Equal, query);
                }
            }
            return sourceQuery;
        }
    }
}
