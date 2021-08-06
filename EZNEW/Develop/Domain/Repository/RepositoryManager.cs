using System;
using System.Collections.Generic;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Repository manager
    /// </summary>
    internal static class RepositoryManager
    {
        #region IQuery handler

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

        #endregion

        #region Global condition

        #region Append Repository Condition

        /// <summary>
        /// Append repository condition
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="originalQuery">Origin query</param>
        /// <returns>Return the newest query object</returns>
        internal static IQuery  AppendRepositoryCondition(Type entityType,IQuery originalQuery, QueryUsageScene usageScene)
        {
            if (originalQuery == null)
            {
                originalQuery = QueryManager.Create();
                originalQuery.SetEntityType(entityType);
            }
            else
            {
                originalQuery.SetEntityType(entityType);
            }

            //primary query
            GlobalConditionFilter conditionFilter = new GlobalConditionFilter()
            {
                OriginalQuery = originalQuery,
                UsageSceneEntityType = entityType,
                EntityType = entityType,
                SourceType = QuerySourceType.Repository,
                UsageScene = usageScene
            };
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(originalQuery);
            }
            //subqueries
            if (!originalQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in originalQuery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join
            if (!originalQuery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in originalQuery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
            return originalQuery;
        }

        #endregion

        #region Append Subqueries Condition

        /// <summary>
        /// Append subqueries condition
        /// </summary>
        /// <param name="subquery">Subquery</param>
        /// <param name="conditionFilter">Condition filter</param>
        static void AppendSubqueryCondition(IQuery subquery, GlobalConditionFilter conditionFilter)
        {
            if (subquery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.Subuery;
            conditionFilter.EntityType = subquery.GetEntityType();
            conditionFilter.OriginalQuery = subquery;
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(subquery);
            }
            //subqueries
            if (!subquery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in subquery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join
            if (!subquery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in subquery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
        }

        #endregion

        #region Append Join Condition

        /// <summary>
        /// Append join query condition
        /// </summary>
        /// <param name="joinQuery">Join query</param>
        /// <param name="conditionFilter">Condition filter</param>
        static void AppendJoinQueryCondition(IQuery joinQuery, GlobalConditionFilter conditionFilter)
        {
            if (joinQuery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.JoinQuery;
            conditionFilter.EntityType = joinQuery.GetEntityType();
            conditionFilter.OriginalQuery = joinQuery;
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(joinQuery);
            }
            //subqueries
            if (!joinQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in joinQuery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join query
            if (!joinQuery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in joinQuery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
        }

        #endregion

        #endregion
    }
}
