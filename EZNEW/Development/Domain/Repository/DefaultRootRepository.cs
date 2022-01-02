using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository.Event;
using EZNEW.Development.UnitOfWork;
using EZNEW.Exceptions;
using EZNEW.Paging;
using System.Linq.Expressions;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines default root repository
    /// </summary>
    /// <typeparam name="TModel">Model object</typeparam>
    public abstract class DefaultRootRepository<TModel> : BaseRepository<TModel> where TModel : class, IModel<TModel>
    {
        #region Impl methods

        #region Save

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override TModel Save(TModel @object, ActivationOptions activationOptions = null)
        {
            return Save(new TModel[1] { @object }, activationOptions)?.FirstOrDefault();
        }

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override List<TModel> Save(IEnumerable<TModel> objects, ActivationOptions activationOptions = null)
        {
            return SaveAsync(objects, activationOptions).Result;
        }

        /// <summary>
        /// Save object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task<TModel> SaveAsync(TModel @object, ActivationOptions activationOptions = null)
        {
            return (await SaveAsync(new TModel[1] { @object }, activationOptions).ConfigureAwait(false)).FirstOrDefault();
        }

        /// <summary>
        /// Save objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task<List<TModel>> SaveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null)
        {
            if (objects.IsNullOrEmpty())
            {
                throw new EZNEWException($"{nameof(objects)} is null or empty");
            }
            var records = new List<IActivationRecord>();
            var resultObjects = new List<TModel>();
            var warehouseObjects = objects.Where(c => !c.IdentityValueIsNull());
            if (!warehouseObjects.IsNullOrEmpty())
            {
                warehouseObjects = await GetObjectListByCurrentAsync(warehouseObjects, true, true).ConfigureAwait(false);
            }
            foreach (var objectItem in objects)
            {
                if (objectItem == null)
                {
                    continue;
                }
                var saveObject = objectItem;
                string saveModelIdentityValue = saveObject.GetIdentityValue();
                bool isAdd = true;
                if (!saveObject.IdentityValueIsNull())
                {
                    var warehouseModel = warehouseObjects?.FirstOrDefault(c => c.GetIdentityValue() == saveModelIdentityValue);
                    if (warehouseModel != null)
                    {
                        saveObject = warehouseModel.OnDataUpdating(saveObject) as TModel;
                        isAdd = false;
                        if (warehouseModel.GetIdentityValue() != saveModelIdentityValue)
                        {
                            throw new EZNEWException("Object identifiers are not allowed to be modified");
                        }
                    }
                }
                if (isAdd)
                {
                    saveObject = saveObject.OnDataAdding() as TModel;
                }
                if (!saveObject.AllowToSave())
                {
                    throw new EZNEWException($"{typeof(TModel).Name} data:{saveModelIdentityValue} cann't to be save");
                }
                var record = ExecuteSaving(saveObject, activationOptions);
                if (record != null)
                {
                    records.Add(record);
                    resultObjects.Add(saveObject);
                }
            }
            RepositoryEventBus.PublishSave(GetType(), resultObjects, activationOptions);
            WorkManager.RegisterActivationRecord(records);
            return resultObjects;
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(TModel @object, ActivationOptions activationOptions = null)
        {
            Remove(new TModel[1] { @object }, activationOptions);
        }

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IEnumerable<TModel> objects, ActivationOptions activationOptions = null)
        {
            RemoveAsync(objects, activationOptions).Wait();
        }

        /// <summary>
        /// Remove by relation object
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null)
        {
            if (relationObjects.IsNullOrEmpty())
            {
                return;
            }
            var removeQuery = GetQueryByRelationData(relationObjects);
            Remove(removeQuery, activationOptions);
        }

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveAsync(TModel @object, ActivationOptions activationOptions = null)
        {
            await RemoveAsync(new TModel[1] { @object }, activationOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove objects
        /// </summary>
        /// <param name="objects">Model objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveAsync(IEnumerable<TModel> objects, ActivationOptions activationOptions = null)
        {
            if (objects.IsNullOrEmpty())
            {
                throw new EZNEWException($"{nameof(objects)} is null or empty");
            }
            var records = new List<IActivationRecord>();
            if (!(activationOptions?.ForceExecution ?? false))
            {
                await GetObjectListByCurrentAsync(objects, true, false).ConfigureAwait(false);
            }
            foreach (var objectItem in objects)
            {
                if (objectItem == null)
                {
                    throw new EZNEWException("Object is null");
                }
                if (!objectItem.AllowToRemove())
                {
                    throw new EZNEWException($"{typeof(TModel)} object:{objectItem.GetIdentityValue()} cann't to be remove");
                }
                var record = ExecuteRemoving(objectItem, activationOptions);//Execute remove
                if (record != null)
                {
                    records.Add(record);
                }
            }
            RepositoryEventBus.PublishRemove(GetType(), (activationOptions?.ForceExecution ?? false) ? objects : objects.Where(o => !o.IsNew()), activationOptions);
            WorkManager.RegisterActivationRecord(records);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects, ActivationOptions activationOptions = null)
        {
            if (relationObjects.IsNullOrEmpty())
            {
                return;
            }
            var removeQuery = GetQueryByRelationData(relationObjects);
            await RemoveAsync(removeQuery, activationOptions).ConfigureAwait(false);
        }

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            RemoveAsync(query, activationOptions).Wait();
        }

        /// <summary>
        /// Remove object by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null)
        {
            RemoveAsync(conditionExpression, activationOptions).Wait();
        }

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationDataQuery">Relation data query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByRelationData(IQuery relationDataQuery, ActivationOptions activationOptions = null)
        {
            RemoveByRelationDataAsync(relationDataQuery, activationOptions).Wait();
        }

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveAsync(IQuery query, ActivationOptions activationOptions = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Remove, AppendRemovingCondition);
            var record = ExecuteRemoving(newQuery, activationOptions);
            if (record != null)
            {
                RepositoryEventBus.PublishRemove<TModel>(GetType(), newQuery, activationOptions);
                WorkManager.RegisterActivationRecord(record);
                RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Remove);
            }
            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveAsync(Expression<Func<TModel, bool>> conditionExpression, ActivationOptions activationOptions = null)
        {
            await RemoveAsync(QueryManager.Create(conditionExpression), activationOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Remove by relation data
        /// </summary>
        /// <param name="relationDataQuery">Relation data query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task RemoveByRelationDataAsync(IQuery relationDataQuery, ActivationOptions activationOptions = null)
        {
            if (relationDataQuery == null)
            {
                return;
            }
            var removeQuery = GetQueryByRelationDataQuery(relationDataQuery);
            Remove(removeQuery, activationOptions);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Modify(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null)
        {
            ModifyAsync(modificationExpression, query, activationOptions).Wait();
        }

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override async Task ModifyAsync(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Modify, AppendModificationCondition);
            var record = ExecuteModification(modificationExpression, newQuery, activationOptions);
            if (record != null)
            {
                RepositoryEventBus.PublishModify<TModel>(GetType(), modificationExpression, newQuery, activationOptions);
                WorkManager.RegisterActivationRecord(record);
                RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Modify);
            }
            await Task.CompletedTask.ConfigureAwait(false);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object data</returns>
        public sealed override TModel Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object data</returns>
        public sealed override TModel Get(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Get(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get by current object
        /// </summary>
        /// <param name="currentObject">Current data</param>
        /// <returns>Return object data</returns>
        public sealed override TModel GetByCurrent(TModel currentObject)
        {
            return GetByCurrentAsync(currentObject).Result;
        }

        /// <summary>
        /// Get by current object
        /// </summary>
        /// <param name="currentObject">Current object</param>
        /// <returns>Return object data</returns>
        public sealed override async Task<TModel> GetByCurrentAsync(TModel currentObject)
        {
            return (await GetListByCurrentAsync(new TModel[1] { currentObject }).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object data</returns>
        public sealed override async Task<TModel> GetAsync(IQuery query)
        {
            return (await GetListAsync(query).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Get object
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object data</returns>
        public sealed override async Task<TModel> GetAsync(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await GetAsync(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Get list

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object list</returns>
        public sealed override List<TModel> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object list</returns>
        public sealed override List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression)
        {
            return GetList(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Gets list by current objects
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <returns>Return object list</returns>
        public sealed override List<TModel> GetListByCurrent(IEnumerable<TModel> currentObjects)
        {
            return GetListByCurrentAsync(currentObjects).Result;
        }

        /// <summary>
        /// Get list by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return object list</returns>
        public sealed override List<TModel> GetListByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects)
        {
            return GetListByRelationDataAsync(relationObjects).Result;
        }

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object list</returns>
        public sealed override async Task<List<TModel>> GetListAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Query, AppendQueryingCondition);
            var datas = await GetObjectListAsync(newQuery).ConfigureAwait(false);
            QueryCallback(newQuery, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, newQuery, result =>
            {
                QueryEventResult<TModel> queryResult = result as QueryEventResult<TModel>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Query);
            return datas ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return object list</returns>
        public sealed override async Task<List<TModel>> GetListAsync(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await GetListAsync(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets list by current objects
        /// </summary>
        /// <param name="currentObjects">Current datas</param>
        /// <returns>Return object list</returns>
        public sealed override async Task<List<TModel>> GetListByCurrentAsync(IEnumerable<TModel> currentObjects)
        {
            return await GetObjectListByCurrentAsync(currentObjects).ConfigureAwait(false);
        }

        /// <summary>
        /// Get list by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation datas</param>
        /// <returns>Return objects</returns>
        public sealed override async Task<List<TModel>> GetListByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects)
        {
            if (relationObjects.IsNullOrEmpty())
            {
                return new List<TModel>(0);
            }
            var query = GetQueryByRelationData(relationObjects);
            return await GetListAsync(query).ConfigureAwait(false);
        }

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression)
        {
            return GetPaging(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return paging data</returns>
        public sealed override async Task<PagingInfo<TModel>> GetPagingAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Query, AppendQueryingCondition);
            if (query.PagingInfo == null)
            {
                query.SetPaging(1);
            }
            var paging = await GetObjectPagingAsync(newQuery).ConfigureAwait(false);
            IEnumerable<TModel> datas = paging?.Items ?? Array.Empty<TModel>();
            QueryCallback(newQuery, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, newQuery, result =>
            {
                QueryEventResult<TModel> queryResult = result as QueryEventResult<TModel>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Query);
            return Pager.Create(paging.Page, paging.PageSize, paging.TotalCount, datas);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return paging data</returns>
        public sealed override async Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await GetPagingAsync(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        /// <summary>
        /// Get paging by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return paging data</returns>
        public sealed override PagingInfo<TModel> GetPagingByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationObjects)
        {
            return GetPagingByRelationDataAsync(relationObjects).Result;
        }

        /// <summary>
        /// Get paging by relation objects
        /// </summary>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return paging data</returns>
        public sealed override async Task<PagingInfo<TModel>> GetPagingByRelationDataAsync<TRelationModel>(IEnumerable<TRelationModel> relationObjects)
        {
            if (relationObjects.IsNullOrEmpty())
            {
                return PagingInfo<TModel>.Empty();
            }
            var query = GetQueryByRelationData(relationObjects);
            return await GetPagingAsync(query).ConfigureAwait(false);
        }

        #endregion

        #region Exists

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        public sealed override bool Exists(IQuery query)
        {
            return ExistsAsync(query).Result;
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether exists data</returns>
        public sealed override bool Exists(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Exists(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        public sealed override async Task<bool> ExistsAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Exists, AppendExistsCondition);
            var existValue = await ExistsDataAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Exists);
            return existValue;
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return whether exists data</returns>
        public sealed override async Task<bool> ExistsAsync(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await ExistsAsync(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Count

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public sealed override long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        public sealed override long Count(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Count(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public sealed override async Task<long> CountAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Count, AppendCountCondition);
            var countValue = await CountValueAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Count);
            return countValue;
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return data count</returns>
        public sealed override async Task<long> CountAsync(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await CountAsync(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Max value

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public sealed override TValue Max<TValue>(IQuery query)
        {
            return MaxAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        public sealed override TValue Max<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Max<TValue>(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public sealed override async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Max, AppendMaxCondition);
            var maxValue = await MaxValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Max);
            return maxValue;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the max value</returns>
        public sealed override async Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await MaxAsync<TValue>(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Min value

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public sealed override TValue Min<TValue>(IQuery query)
        {
            return MinAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        public sealed override TValue Min<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Min<TValue>(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public sealed override async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Min, AppendMinCondition);
            var minValue = await MinValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Min);
            return minValue;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the min value</returns>
        public sealed override async Task<TValue> MinAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await MinAsync<TValue>(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Sum value

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public sealed override TValue Sum<TValue>(IQuery query)
        {
            return SumAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        public sealed override TValue Sum<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Sum<TValue>(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public sealed override async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Sum, AppendSumCondition);
            var sumValue = await SumValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Sum);
            return sumValue;
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the sum value</returns>
        public sealed override async Task<TValue> SumAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await SumAsync<TValue>(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #region Average value

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return average value</returns>
        public sealed override TValue Avg<TValue>(IQuery query)
        {
            return AvgAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        public sealed override TValue Avg<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return Avg<TValue>(QueryManager.Create(conditionExpression));
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return average value</returns>
        public sealed override async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecution(query, QueryUsageScene.Avg, AppendAvgCondition);
            var avgValue = await AvgValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecution(query, newQuery, QueryUsageScene.Avg);
            return avgValue;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the average value</returns>
        public sealed override async Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, bool>> conditionExpression)
        {
            return await AvgAsync<TValue>(QueryManager.Create(conditionExpression)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Functions

        /// <summary>
        /// Execute saving
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteSaving(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Execute removing
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteRemoving(TModel @object, ActivationOptions activationOptions = null);

        /// <summary>
        /// Execute removing by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteRemoving(IQuery query, ActivationOptions activationOptions = null);

        ///// <summary>
        ///// Get object
        ///// </summary>
        ///// <param name="query">Query object</param>
        ///// <returns>Return data</returns>
        //protected abstract Task<TModel> GetModelAsync(IQuery query);

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        protected abstract Task<List<TModel>> GetObjectListAsync(IQuery query);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        protected abstract Task<PagingInfo<TModel>> GetObjectPagingAsync(IQuery query);

        /// <summary>
        /// Get objectlist by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <param name="includeRemove">Indicates whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns>Return object list</returns>
        protected abstract Task<List<TModel>> GetObjectListByCurrentAsync(IEnumerable<TModel> currentObjects, bool includeRemove = false, bool onlyCompleteObject = false);

        /// <summary>
        /// Indicates whether has data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Whether has data</returns>
        protected abstract Task<bool> ExistsDataAsync(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count value</returns>
        protected abstract Task<long> CountValueAsync(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected abstract Task<TValue> MaxValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected abstract Task<TValue> MinValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected abstract Task<TValue> SumValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        protected abstract Task<TValue> AvgValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute modification
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteModification(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Query callback
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="batchReturn">Whether batch return</param>
        /// <param name="datas">Datas</param>
        protected virtual void QueryCallback(IQuery query, bool batchReturn, IEnumerable<TModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            foreach (var data in datas)
            {
                if (data == null)
                {
                    continue;
                }
                if (batchReturn)
                {
                    data.CloseLazyMember();
                }
                if (!(query?.LoadProperties.IsNullOrEmpty() ?? true))
                {
                    data.SetLoadProperties(query.LoadProperties);
                }
            }
        }

        /// <summary>
        /// Get query by relation object
        /// </summary>
        /// <typeparam name="TRelationData">Relation data types</typeparam>
        /// <param name="relationObjects">Relation objects</param>
        /// <returns>Return a IQuery object</returns>
        protected abstract IQuery GetQueryByRelationData<TRelationData>(IEnumerable<TRelationData> relationObjects);

        /// <summary>
        /// Get query by relation object query
        /// </summary>
        /// <param name="relationObjectQuery">Relation object query</param>
        /// <returns>Return a IQuery object</returns>
        protected abstract IQuery GetQueryByRelationDataQuery(IQuery relationObjectQuery);

        #endregion

        #region Global condition

        #region Append removing extra condition

        /// <summary>
        /// Append removig condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendRemovingCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append modification extra condition

        /// <summary>
        /// Append modification condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendModificationCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append querying extra condition

        /// <summary>
        /// Append querying condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendQueryingCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append exists extra condition

        /// <summary>
        /// Append exists condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendExistsCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append count extra condition

        /// <summary>
        /// Append count condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendCountCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append max extra condition

        /// <summary>
        /// Append max condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendMaxCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append min extra condition

        /// <summary>
        /// Append min condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendMinCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append sum extra condition

        /// <summary>
        /// Append sum condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendSumCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append avg extra condition

        /// <summary>
        /// Append avg condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendAvgCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #endregion
    }
}
