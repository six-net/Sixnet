using System;
using System.Collections.Generic;
using EZNEW.DependencyInjection;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Aggregation;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
using EZNEW.Exceptions;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Repository manager
    /// </summary>
    public static class RepositoryManager
    {
        /// <summary>
        /// Default repositories
        /// </summary>
        readonly static Dictionary<Guid, Type> DefaultRepositories = new Dictionary<Guid, Type>();

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

        #region Default repository

        internal static void RegisterDefaultRepository(Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                return;
            }
            Type entityDataAccessType = DataAccessManager.GetDataAccessType(entityType);
            if (entityDataAccessType == null)
            {
                return;
            }
            Type entityRepositoryInterfaceType = typeof(IRepository<>).MakeGenericType(entityType); ;
            Type entityReposirotyType = typeof(DefaultRepository<,,>).MakeGenericType(entityType, entityType, entityDataAccessType);

            if (entityRepositoryInterfaceType != null && entityReposirotyType != null)
            {
                ContainerManager.AddInternalService(entityRepositoryInterfaceType, entityReposirotyType);
                DefaultRepositories[entityType.GUID] = entityRepositoryInterfaceType;
            }

        }

        /// <summary>
        /// Get repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<TModel> GetRepository<TModel>()
            where TModel : IAggregationRoot<TModel>
        {
            return ContainerManager.Resolve<IRepository<TModel>>();
        }

        #endregion
    }
}
