using System;
using System.Collections.Generic;
using EZNEW.DependencyInjection;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Repository manager
    /// </summary>
    public static class RepositoryManager
    {
        #region IQuery handler

        /// <summary>
        /// Handle IQuery before execution
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="usageScene">Usage scene</param>
        /// <param name="queryHandler">Query handler</param>
        /// <returns>Return the real query object to use</returns>
        internal static IQuery HandleQueryObjectBeforeExecution(IQuery query, QueryUsageScene usageScene, Func<IQuery, IQuery> queryHandler = null)
        {
            var newQuery = query?.Clone();
            if (queryHandler != null)
            {
                newQuery = queryHandler(newQuery);
            }
            return newQuery;
        }

        /// <summary>
        /// Handle IQuery after execution
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="executeQuery">Execute query</param>
        /// <param name="usageScene">Usage scene</param>
        internal static void HandleQueryObjectAfterExecution(IQuery originalQuery, IQuery executeQuery, QueryUsageScene usageScene)
        {
            originalQuery?.Reset();
        }

        #endregion

        #region Default repository

        /// <summary>
        /// Register default repository
        /// </summary>
        /// <param name="modelType">Model type</param>
        internal static void RegisterDefaultRepository(Type modelType)
        {
            if (modelType == null)
            {
                return;
            }
            var entityType = ModelManager.GetModelRelationEntityType(modelType);
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
            Type modelRepositoryInterfaceType = typeof(IRepository<>).MakeGenericType(modelType);
            Type modelReposirotyType = typeof(DefaultRepository<,,>).MakeGenericType(modelType, entityType, entityDataAccessType);
            if (modelRepositoryInterfaceType != null && modelReposirotyType != null)
            {
                ContainerManager.AddDefaultProjectService(modelRepositoryInterfaceType, modelReposirotyType);
            }

        }

        /// <summary>
        /// Get model default repository
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns>Return model default repository</returns>
        internal static Type GetDefaultRepository(Type modelType)
        {
            if (modelType == null)
            {
                return null;
            }
            return typeof(IRepository<>).MakeGenericType(modelType);
        }

        /// <summary>
        /// Get repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRepository<TModel> GetRepository<TModel>()
            where TModel : IModel<TModel>
        {
            return ContainerManager.Resolve<IRepository<TModel>>();
        }

        #endregion
    }
}
