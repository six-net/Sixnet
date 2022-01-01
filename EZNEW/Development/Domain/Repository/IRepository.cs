using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.UnitOfWork;
using EZNEW.Development.Query;
using EZNEW.Paging;
using EZNEW.Development.Domain.Model;
using System.Linq.Expressions;
using System;

namespace EZNEW.Development.Domain.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Remove by relation object
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationDataQuery">Relation data query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByRelationData(IQuery relationDataQuery, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation object
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationDataQuery">Relation data query object</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveByRelationDataAsync(IQuery relationDataQuery, ActivationOptions activationOptions = null);
    }

    /// <summary>
    /// Defines repository contract
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public interface IRepository<TModel> : IRepository where TModel : IModel<TModel>
    {
        #region Save

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="activationOptions">Activation options</param>
        TModel Save(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="objects">Objects</param>
        /// <param name="activationOptions">Activation options</param>
        List<TModel> Save(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="activationOptions">Activation options</param>
        Task<TModel> SaveAsync(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="objects">Objects</param>
        /// <param name="activationOptions">Activation options</param>
        Task<List<TModel>> SaveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Objects</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveAsync(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Objects</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null);

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveAsync(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        Task RemoveAsync(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null);

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="expression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void Modify(IModification expression, IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="expression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        Task ModifyAsync(IModification expression, IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Get

        /// <summary>
        /// Get model object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Model object</returns>
        TModel Get(IQuery query);

        /// <summary>
        /// Get model object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Model object</returns>
        TModel Get(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get by current object
        /// </summary>
        /// <param name="currentObject">Current object</param>
        /// <returns>Model object</returns>
        TModel GetByCurrent(TModel currentObject);

        /// <summary>
        /// Get model object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Model object</returns>
        Task<TModel> GetAsync(IQuery query);

        /// <summary>
        /// Get model object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Model object</returns>
        Task<TModel> GetAsync(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get by current object
        /// </summary>
        /// <param name="currentObject">Current model object</param>
        /// <returns>Model object</returns>
        Task<TModel> GetByCurrentAsync(TModel currentObject);

        #endregion

        #region Get list

        /// <summary>
        /// Get model object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return model object list</returns>
        List<TModel> GetList(IQuery query);

        /// <summary>
        /// Get model object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return model object list</returns>
        List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get list by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <returns>Return model object list</returns>
        List<TModel> GetListByCurrent(IEnumerable<TModel> currentObjects);

        /// <summary>
        /// Get list by relation data
        /// </summary>
        /// <param name="relationObjects">Relation models</param>
        /// <returns>Return model list</returns>
        List<TModel> GetListByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        /// <summary>
        /// Get model object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return model object list</returns>
        Task<List<TModel>> GetListAsync(IQuery query);

        /// <summary>
        /// Get model object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return model object list</returns>
        Task<List<TModel>> GetListAsync(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get list by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <returns>Return model object list</returns>
        Task<List<TModel>> GetListByCurrentAsync(IEnumerable<TModel> currentObjects);

        /// <summary>
        /// Get list by relation models
        /// </summary>
        /// <param name="relationObjects">Relation models</param>
        /// <returns>Return model object list</returns>
        Task<List<TModel>> GetListByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        PagingInfo<TModel> GetPaging(IQuery query);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get paging by relation data
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return paging data</returns>
        PagingInfo<TModel> GetPagingByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        Task<PagingInfo<TModel>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Get paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get list by relation objects
        /// </summary>
        /// <param name="relationObects">Relation objects</param>
        /// <returns>Return paging data</returns>
        Task<PagingInfo<TModel>> GetPagingByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObects);

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        bool Exists(IQuery query);

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether exists data</returns>
        bool Exists(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        Task<bool> ExistsAsync(IQuery query);

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether exists data</returns>
        Task<bool> ExistsAsync(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get data count

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        long Count(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        long Count(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        Task<long> CountAsync(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get max value

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        TValue Max<TValue>(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        TValue Max<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        Task<TValue> MaxAsync<TValue>(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get min value

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return min value</returns>
        TValue Min<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        TValue Min<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return min value</returns>
        Task<TValue> MinAsync<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        Task<TValue> MinAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get sum value

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        TValue Sum<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        TValue Sum<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        Task<TValue> SumAsync<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        Task<TValue> SumAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get average value

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        TValue Avg<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        TValue Avg<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        Task<TValue> AvgAsync<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression);

        #endregion

        #region Get data source

        /// <summary>
        /// Get data source
        /// </summary>
        /// <param name="object">Object</param>
        /// <returns>Return data source</returns>
        DataSource GetDataSource(IModel @object);

        #endregion

        #region Modify data source

        /// <summary>
        /// Modify data source
        /// </summary>
        /// <param name="object">Object</param>
        /// <param name="source">Source</param>
        void ModifyDataSource(IModel @object, DataSource source);

        #endregion
    }
}
