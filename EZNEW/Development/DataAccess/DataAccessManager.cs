using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Application;
using EZNEW.Data.Cache;
using EZNEW.DependencyInjection;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Entity;

namespace EZNEW.Development.DataAccess
{
    /// <summary>
    /// Defines data access manager
    /// </summary>
    public static class DataAccessManager
    {
        static readonly Dictionary<Guid, Type> EntityDataAccessCollection = new();

        #region Register entity default data access

        /// <summary>
        /// Register entity default data access
        /// </summary>
        /// <param name="entityType">Entity type</param>
        internal static void RegisterEntityDefaultDataAccess(Type entityType)
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
            Type dataAccessInterface = typeof(IDataAccess<>).MakeGenericType(entityType);
            Type dataAccessType = typeof(DefaultDataAccess<>).MakeGenericType(entityType);
            ContainerManager.AddDefaultProjectService(dataAccessInterface, dataAccessType);
            Type entityDataAccessInterface = dataAccessInterface;
            if (entityConfig.EnableCache)
            {
                var cacheAccessInterface = typeof(ICacheDataAccess<>).MakeGenericType(entityType);
                var cacheAccessType = typeof(DefaultCacheDataAccess<,>).MakeGenericType(dataAccessInterface, entityType);
                ContainerManager.AddDefaultProjectService(cacheAccessInterface, cacheAccessType);
                entityDataAccessInterface = cacheAccessInterface;
            }
            EntityDataAccessCollection[entityType.GUID] = entityDataAccessInterface;

            //warehouse
            ContainerManager.AddWarehouseService(entityType, entityDataAccessInterface);
        }

        #endregion

        #region Get data access

        /// <summary>
        /// Get data access
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return a data access object</returns>
        public static IDataAccess<TEntity> GetDataAccess<TEntity>() where TEntity : BaseEntity<TEntity>, new()
        {
            var dataAccessType = GetDataAccessType(typeof(TEntity));
            if (dataAccessType != null)
            {
                return ContainerManager.Resolve(dataAccessType) as IDataAccess<TEntity>;
            }
            return null;
        }

        #endregion

        #region Get data access type

        /// <summary>
        /// Get data access type
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return data access type</returns>
        public static Type GetDataAccessType(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            EntityDataAccessCollection.TryGetValue(entityType.GUID, out var dataAccessType);
            return dataAccessType;
        }

        #endregion
    }
}
