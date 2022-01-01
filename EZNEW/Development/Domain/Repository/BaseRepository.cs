using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.UnitOfWork;
using EZNEW.Paging;
using System.Linq.Expressions;
using System;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines base repository
    /// </summary>
    public abstract class BaseRepository<TModel> : IRepository<TModel> where TModel : IModel<TModel>
    {
        #region Save

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract TModel Save(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract List<TModel> Save(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task<TModel> SaveAsync(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task<List<TModel>> SaveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation object
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveAsync(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation object
        /// </summary>
        /// <param name="relationObjects">Relation datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null);

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove data by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove data by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation object query
        /// </summary>
        /// <param name="relationModelQuery">Relation object query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByRelationData(IQuery relationModelQuery, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveAsync(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveAsync(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationModelQuery">Relation object query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task RemoveByRelationDataAsync(IQuery relationModelQuery, ActivationOptions activationOptions = null);

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Modify(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="expression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract Task ModifyAsync(IModification expression, IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Get

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object data</returns>
        public abstract TModel Get(IQuery query);

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object data</returns>
        public abstract TModel Get(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get by current object
        /// </summary>
        /// <param name="currentObject">Current object</param>
        /// <returns>Return object data</returns>
        public abstract TModel GetByCurrent(TModel currentObject);

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="currentData">Current data</param>
        /// <returns>Return data</returns>
        public abstract Task<TModel> GetByCurrentAsync(TModel currentData);

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object</returns>
        public abstract Task<TModel> GetAsync(IQuery query);

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object data</returns>
        public abstract Task<TModel> GetAsync(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get list

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object list</returns>
        public abstract List<TModel> GetList(IQuery query);

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data list</returns>
        public abstract List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Gets list by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <returns>Return object list</returns>
        public abstract List<TModel> GetListByCurrent(IEnumerable<TModel> currentObjects);

        /// <summary>
        /// Get list by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return object list</returns>
        public abstract List<TModel> GetListByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public abstract Task<List<TModel>> GetListAsync(IQuery query);

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object list</returns>
        public abstract Task<List<TModel>> GetListAsync(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Gets list by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <returns>Return object list</returns>
        public abstract Task<List<TModel>> GetListByCurrentAsync(IEnumerable<TModel> currentObjects);

        /// <summary>
        /// Get list by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TModel>> GetListByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        public abstract PagingInfo<TModel> GetPaging(IQuery query);

        /// <summary>
        /// Get paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        public abstract PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get paging data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Get paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get paging by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return paging data</returns>
        public abstract PagingInfo<TModel> GetPagingByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        /// <summary>
        /// Get paging by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether has data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether has data</returns>
        public abstract bool Exists(IQuery query);

        /// <summary>
        /// Determines whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether has data</returns>
        public abstract bool Exists(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Determines whether has data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether has data</returns>
        public abstract Task<bool> ExistsAsync(IQuery query);

        /// <summary>
        /// Determines whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether has data</returns>
        public abstract Task<bool> ExistsAsync(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get data count

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public abstract long Count(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        public abstract long Count(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public abstract Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        public abstract Task<long> CountAsync(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get max value

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public abstract TValue Max<TValue>(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        public abstract TValue Max<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public abstract Task<TValue> MaxAsync<TValue>(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        public abstract Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get min value

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public abstract TValue Min<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        public abstract TValue Min<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public abstract Task<TValue> MinAsync<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        public abstract Task<TValue> MinAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get sum value

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public abstract TValue Sum<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        public abstract TValue Sum<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public abstract Task<TValue> SumAsync<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        public abstract Task<TValue> SumAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get average value

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        public abstract TValue Avg<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        public abstract TValue Avg<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        public abstract Task<TValue> AvgAsync<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        public abstract Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get data source

        /// <summary>
        /// Get object data source
        /// </summary>
        /// <param name="object">Model object</param>
        /// <returns>Return the data life source</returns>
        public abstract DataSource GetDataSource(IModel @object);

        #endregion

        #region Modify data source

        /// <summary>
        /// Modify data source
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="source">Life source</param>
        public abstract void ModifyDataSource(IModel @object, DataSource source);

        #endregion
    }
}
