using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Entity;
using EZNEW.Development.UnitOfWork;
using EZNEW.Paging;
using EZNEW.Development.Domain.Repository.Warehouse.Storage;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines entity warehouse contract
    /// </summary>
    public interface IEntityWarehouse<TEntity, TDataAccess> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        #region Save

        /// <summary>
        /// Save entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Save(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Save(TEntity entity, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Remove(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Remove(TEntity entity, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Remove(IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Modify

        /// <summary>
        /// Modification data by expression
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        IActivationRecord Modify(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Query

        /// <summary>
        /// Get an entity
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Entity data</returns>
        Task<TEntity> GetAsync(IQuery query);

        /// <summary>
        /// Query an entity list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Entity list</returns>
        Task<List<TEntity>> GetListAsync(IQuery query);

        /// <summary>
        /// Get warehouse entities
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        Task<List<TEntity>> GetWarehouseEntitiesAsync(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false);

        /// <summary>
        /// Get warehouse entity packages
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        Task<List<EntityPackage<TEntity>>> GetWarehouseEntityPackagesAsync(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false);

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Entity paging</returns>
        Task<PagingInfo<TEntity>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Whether exists data</returns>
        Task<bool> ExistsAsync(IQuery query);

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Count value</returns>
        Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Max value</returns>
        Task<TValue> MaxAsync<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Min value</returns>
        Task<TValue> MinAsync<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Sum value</returns>
        Task<TValue> SumAsync<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Average value</returns>
        Task<TValue> AvgAsync<TValue>(IQuery query);

        #endregion

        #region Source

        /// <summary>
        /// Get entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity source</returns>
        DataSource GetEntitySource(TEntity entity);

        /// <summary>
        /// Modify entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="source">Source</param>
        void ModifyEntitySource(TEntity entity, DataSource source);

        #endregion
    }
}
