using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Cache;
using EZNEW.Cache.Keys;
using EZNEW.Cache.String;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Logging;
using EZNEW.Queue;
using EZNEW.Selection;
using EZNEW.Serialize;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Default data cache policy
    /// </summary>
    public class DefaultDataCachePolicy : IDataCachePolicy
    {
        private static readonly DataCachePolicyService dataCachePolicyService = new DataCachePolicyService();
        private static readonly ShuffleNet<int> randomSecondPrivider = new ShuffleNet<int>(Enumerable.Range(1, 10), true, true);

        #region Starting

        /// <summary>
        /// Called before a database command execute
        /// </summary>
        /// <param name="addDataContext">Add data context</param>
        /// <returns>Return the policy result</returns>
        public virtual StartingResult OnAddStarting<T>(AddDataContext<T> addDataContext) where T : BaseEntity<T>, new()
        {
            if (addDataContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(addDataContext)} is null");
            }
            try
            {
                AddCacheData(new QueryDataCallbackContext<T>()
                {
                    Datas = addDataContext.Datas,
                    Query = addDataContext.DatabaseCommand?.Query
                });
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.AddData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeDataContext">Remove data context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnRemoveStarting<T>(RemoveDataContext<T> removeDataContext) where T : BaseEntity<T>, new()
        {
            if (removeDataContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(removeDataContext)} is null");
            }
            try
            {
                RemoveCacheData(removeDataContext.Datas);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeByQueryContext">Remove by query context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnRemoveByQueryStarting<T>(RemoveByQueryContext<T> removeByQueryContext) where T : BaseEntity<T>, new()
        {
            if (removeByQueryContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(removeByQueryContext)} is null");
            }
            try
            {
                RemoveCacheData(removeByQueryContext.Datas);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeAllContext">Remove all context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnRemoveAllStarting<T>(RemoveAllContext<T> removeAllContext) where T : BaseEntity<T>, new()
        {
            if (removeAllContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(removeAllContext)} is null");
            }
            var entityType = typeof(T);
            try
            {
                RemoveCacheDataByType(entityType);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateByQueryContext">Update by query context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnUpdateByQueryStarting<T>(UpdateByQueryContext<T> updateByQueryContext) where T : BaseEntity<T>, new()
        {
            if (updateByQueryContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(updateByQueryContext)} is null");
            }
            if (updateByQueryContext.GetDatasProxy == null)
            {
                return StartingResult.Success($"Parameter:{nameof(UpdateByQueryContext<T>.GetDatasProxy)} is null");
            }
            var datas = updateByQueryContext.GetDatasProxy(updateByQueryContext.Query);
            try
            {
                RemoveCacheData(datas);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateAllContext">Update all context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnUpdateAllStarting<T>(UpdateAllContext<T> updateAllContext) where T : BaseEntity<T>, new()
        {
            if (updateAllContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(updateAllContext)} is null");
            }
            var entityType = typeof(T);
            try
            {
                RemoveCacheDataByType(entityType);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateDataContext">Update data context</param>
        /// <returns>Return policy result</returns>
        public virtual StartingResult OnUpdateStarting<T>(UpdateDataContext<T> updateDataContext) where T : BaseEntity<T>, new()
        {
            if (updateDataContext == null)
            {
                return StartingResult.Success($"Parameter:{nameof(updateDataContext)} is null");
            }
            try
            {
                RemoveCacheData(updateDataContext.Datas);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetStartingResult(DataCacheOperation.RemoveData, ex);
            }
            return StartingResult.Success();
        }

        /// <summary>
        /// Called before a query command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="queryDataContext">Query data context</param>
        /// <returns>Return query result</returns>
        public virtual QueryDataResult<T> OnQueryStarting<T>(QueryDataContext<T> queryDataContext) where T : BaseEntity<T>, new()
        {
            if (queryDataContext == null)
            {
                return QueryDataResult<T>.Default($"Parameter:{nameof(queryDataContext)} is null");
            }
            IQuery query = queryDataContext.Query;
            int size = query?.QuerySize ?? 0;
            try
            {
                return GetCacheDatas<T>(query, size);
            }
            catch (Exception ex)
            {
                return DataCacheBehavior.GetQueryResult<T>(ex);
            }
        }

        #endregion

        #region Callback

        /// <summary>
        /// Called after a add data database command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="addDataContext">Add data context</param>
        public virtual void OnAddCallback<T>(AddDataContext<T> addDataContext) where T : BaseEntity<T>, new()
        {
            if (addDataContext == null)
            {
                return;
            }
            AddCacheData(new QueryDataCallbackContext<T>()
            {
                Datas = addDataContext.Datas,
                Query = addDataContext.DatabaseCommand?.Query
            });
        }

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeDataContext">Remove data context</param>
        public virtual void OnRemoveCallback<T>(RemoveDataContext<T> removeDataContext) where T : BaseEntity<T>, new()
        {
            if (removeDataContext == null)
            {
                return;
            }
            RemoveCacheData(removeDataContext.Datas);
        }

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeByQueryContext">Remove by query context</param>
        public virtual void OnRemoveByQueryCallback<T>(RemoveByQueryContext<T> removeByQueryContext) where T : BaseEntity<T>, new()
        {
            if (removeByQueryContext?.Datas.IsNullOrEmpty() ?? true)
            {
                return;
            }
            RemoveCacheData(removeByQueryContext.Datas);
        }

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeAllContext">Remove all context</param>
        public virtual void OnRemoveAllCallback<T>(RemoveAllContext<T> removeAllContext) where T : BaseEntity<T>, new()
        {
            RemoveCacheDataByType(typeof(T));
        }

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateByQueryContext">Update by query context</param>
        public virtual void OnUpdateByQueryCallback<T>(UpdateByQueryContext<T> updateByQueryContext) where T : BaseEntity<T>, new()
        {
            if (updateByQueryContext?.GetDatasProxy == null)
            {
                return;
            }
            var datas = updateByQueryContext.GetDatasProxy(updateByQueryContext.Query);
            AddCacheData(new QueryDataCallbackContext<T>()
            {
                Datas = datas,
                Query = updateByQueryContext.Query
            });
        }

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateAllContext">Update all context</param>
        public virtual void OnUpdateAllCallback<T>(UpdateAllContext<T> updateAllContext) where T : BaseEntity<T>, new()
        {
            RemoveCacheDataByType(typeof(T));
        }

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateDataContext">Update data context</param>
        public virtual void OnUpdateCallback<T>(UpdateDataContext<T> updateDataContext) where T : BaseEntity<T>, new()
        {
            if (updateDataContext?.Datas.IsNullOrEmpty() ?? true)
            {
                return;
            }
            RemoveCacheData(updateDataContext.Datas);
        }

        /// <summary>
        /// Called after a query command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="queryDataContext">Query data callback context</param>
        public virtual void OnQueryCallback<T>(QueryDataCallbackContext<T> queryDataCallbackContext) where T : BaseEntity<T>, new()
        {
            if (queryDataCallbackContext.Datas.IsNullOrEmpty())
            {
                return;
            }
            var query = queryDataCallbackContext.Query;
            var queryFieldsWithSign = query.GetActuallyQueryFieldsWithSign(typeof(T), true);
            //only store complete data
            if (queryFieldsWithSign?.Item1 ?? false)
            {
                var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.Get);
                if (cacheOperationConfig != null && (cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
                {
                    if (cacheOperationConfig.Synchronous)
                    {
                        AddCacheData(queryDataCallbackContext);
                    }
                    else
                    {
                        var internalMsgItem = new InternalQueueAddCacheDataItem<T>(AddCacheData, queryDataCallbackContext);
                        InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.DataCache).Enqueue(internalMsgItem);
                    }
                }
            }
        }

        #endregion

        #region Util

        #region AddCacheData

        /// <summary>
        /// Add datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="query">Query condition</param>
        /// <param name="datas">Datas</param>
        protected virtual void AddCacheData<T>(QueryDataCallbackContext<T> queryDataCallbackContext) where T : BaseEntity<T>, new()
        {
            if (queryDataCallbackContext == null)
            {
                return;
            }

            #region Add cache data

            var datas = queryDataCallbackContext.Datas;
            List<CacheKey> dataPrimaryKeys = null;
            List<CacheKey> dataOtherKeys = null;
            List<CacheEntry> storeItems = new List<CacheEntry>();
            Type entityType = typeof(T);
            string objectName = entityType.Name;
            var cacheObject = new CacheObject()
            {
                ObjectName = objectName
            };
            int dataCount = 0;
            if (!datas.IsNullOrEmpty())
            {
                dataPrimaryKeys = new List<CacheKey>();
                dataOtherKeys = new List<CacheKey>();
                var entityConfiguration = EntityManager.GetEntityConfiguration(entityType);
                if (entityConfiguration == null)
                {
                    LogManager.LogError<DefaultDataCachePolicy>($"Entity :{entityType.FullName} configuration is null");
                    return;
                }

                //primary keys
                var primaryKeys = entityConfiguration.PrimaryKeys;
                if (primaryKeys.IsNullOrEmpty())
                {
                    LogManager.LogError<DefaultDataCachePolicy>($"Data type：{entityType.FullName} no primary key,unable to set cache data");
                    return;
                }

                //cache keys
                var cacheKeys = entityConfiguration.CacheKeys ?? new List<string>(0);
                cacheKeys = cacheKeys.Except(primaryKeys).ToList();

                //cache prefix keys
                var cachePrefixKeys = entityConfiguration.CachePrefixKeys ?? new List<string>(0);
                foreach (var data in datas)
                {
                    if (data == null)
                    {
                        continue;
                    }
                    dataCount++;
                    bool keyValueSuccess = true;
                    //expiration
                    TimeSpan? dataExpirationValue = DataCacheManager.Configuration.GetExpiration(entityType);
                    DateTimeOffset? dataExpirationTime = null;
                    if (dataExpirationValue != null)
                    {
                        dataExpirationTime = DateTimeOffset.Now.Add(dataExpirationValue.Value).AddSeconds(randomSecondPrivider.TakeNextValues(1).FirstOrDefault());
                    }
                    CacheExpiration dataExpiration = new CacheExpiration()
                    {
                        SlidingExpiration = false,
                        AbsoluteExpiration = dataExpirationTime
                    };
                    //prefix cache keys
                    var dataPrefixKey = new CacheKey();
                    if (!cachePrefixKeys.IsNullOrEmpty())
                    {
                        foreach (var preKey in cachePrefixKeys)
                        {
                            var preKeyVal = data.GetValue(preKey)?.ToString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(preKeyVal))
                            {
                                LogManager.LogError<DefaultDataCachePolicy>($"Data type :{entityType.FullName},Identity value:{data.GetIdentityValue()},Cache prefix key:{preKey},value is null or empty,unable to set cache data");
                                keyValueSuccess = false;
                                break;
                            }
                            dataPrefixKey.AddName(preKey, preKeyVal);
                        }
                        if (!keyValueSuccess)
                        {
                            continue;
                        }
                    }
                    //primary data cache keys
                    var dataCacheKey = new CacheKey(cacheObject, dataPrefixKey);
                    foreach (string pk in primaryKeys)
                    {
                        var pkValue = data.GetValue(pk)?.ToString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(pkValue))
                        {
                            LogManager.LogError<DefaultDataCachePolicy>($"Data type :{entityType.FullName},Identity value:{data.GetIdentityValue()},Primary key:{pk},value is null or empty,unable to set cache data");
                            keyValueSuccess = false;
                            break;
                        }
                        dataCacheKey.AddName(pk, pkValue);
                    }
                    if (!keyValueSuccess)
                    {
                        continue;
                    }
                    string primaryFullCacheKey = dataCacheKey.GetActualKey();
                    if (primaryFullCacheKey.IsNullOrEmpty())
                    {
                        continue;
                    }
                    dataPrimaryKeys.Add(primaryFullCacheKey);
                    storeItems.Add(new CacheEntry()
                    {
                        Key = primaryFullCacheKey,
                        Value = JsonSerializeHelper.ObjectToJson(data),
                        Expiration = dataExpiration
                    });
                    if (!cacheKeys.IsNullOrEmpty())
                    {
                        foreach (string key in cacheKeys)
                        {
                            var otherCacheKey = new CacheKey(cacheObject, dataPrefixKey);
                            var cacheKeyValue = data.GetValue(key)?.ToString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(cacheKeyValue))
                            {
                                LogManager.LogError<DefaultDataCachePolicy>($"Data type :{entityType.FullName},Identity value:{data.GetIdentityValue()},Cache key:{key},value is null or empty,unable to set cache data");
                                keyValueSuccess = false;
                                break;
                            }
                            otherCacheKey.AddName(key, cacheKeyValue?.ToString() ?? string.Empty);
                            dataOtherKeys.Add(otherCacheKey);
                            storeItems.Add(new CacheEntry()
                            {
                                Key = otherCacheKey.GetActualKey(),
                                Value = primaryFullCacheKey,
                                Expiration = dataExpiration
                            });
                        }
                        if (!keyValueSuccess)
                        {
                            continue;
                        }
                    }
                }
            }

            #endregion

            #region Null data

            int querySize = queryDataCallbackContext.Query?.QuerySize ?? 0;
            if (DataCacheManager.Configuration.EnableCacheNullData && (querySize < 1 || dataCount < querySize))
            {
                IEnumerable<CacheKey> queryPrimaryKeys = queryDataCallbackContext.PrimaryCacheKeys;
                IEnumerable<CacheKey> queryOtherKeys = queryDataCallbackContext.OtherCacheKeys;
                string nullDataValue = JsonSerializeHelper.ObjectToJson<T>(null);
                TimeSpan? nullDataExpirationValue = DataCacheManager.Configuration.GetNullDataExpiration(entityType);
                DateTimeOffset? nullDataExpiration = null;
                if (nullDataExpirationValue != null)
                {
                    nullDataExpiration = DateTimeOffset.Now.Add(nullDataExpirationValue.Value);
                }
                CacheExpiration nullDataExp = new CacheExpiration()
                {
                    SlidingExpiration = false,
                    AbsoluteExpiration = nullDataExpiration
                };
                if (!queryPrimaryKeys.IsNullOrEmpty())
                {
                    if (!dataPrimaryKeys.IsNullOrEmpty())
                    {
                        queryPrimaryKeys = queryPrimaryKeys.Except(dataPrimaryKeys);
                    }
                    foreach (var primaryKey in queryPrimaryKeys)
                    {
                        storeItems.Add(new CacheEntry()
                        {
                            Key = primaryKey.GetActualKey(),
                            Value = nullDataValue,
                            Expiration = nullDataExp
                        });
                    }
                }
                if (!queryOtherKeys.IsNullOrEmpty())
                {
                    if (!dataOtherKeys.IsNullOrEmpty())
                    {
                        queryOtherKeys = queryOtherKeys.Except(dataOtherKeys);
                    }
                    if (!queryOtherKeys.IsNullOrEmpty())
                    {
                        storeItems.Add(new CacheEntry()
                        {
                            Key = DataCacheManager.Configuration.NullDataCacheKey,
                            Value = nullDataValue,
                            Expiration = nullDataExp
                        });
                        foreach (var otherKey in queryOtherKeys)
                        {
                            storeItems.Add(new CacheEntry()
                            {
                                Key = otherKey,
                                Value = DataCacheManager.Configuration.NullDataCacheKey,
                                Expiration = nullDataExp
                            });
                        }
                    }
                }
            }

            #endregion

            StringSetOptions option = new StringSetOptions()
            {
                CacheObject = cacheObject,
                Items = storeItems,
                CommandFlags = CacheCommandFlags.FireAndForget
            };
            var cacheResult = CacheManager.String.Set(option);
            if (cacheResult != null && !cacheResult.Responses.IsNullOrEmpty())
            {
                foreach (var response in cacheResult.Responses)
                {
                    if (!string.IsNullOrWhiteSpace(response?.Message))
                    {
                        LogManager.LogInformation<DefaultDataCachePolicy>(response.Message);
                    }
                }
            }
        }

        #endregion

        #region RemoveCacheData

        /// <summary>
        /// Remove cache data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Data list</param>
        protected virtual void RemoveCacheData<T>(IEnumerable<T> datas) where T : BaseEntity<T>, new()
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            Type entityType = typeof(T);
            var entityConfiguration = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfiguration == null)
            {
                LogManager.LogError<DefaultDataCachePolicy>($"Entity :{entityType.FullName} configuration is null");
                return;
            }
            string objectName = entityType.Name;

            //primary keys
            var primaryKeys = entityConfiguration.PrimaryKeys;
            if (primaryKeys.IsNullOrEmpty())
            {
                LogManager.LogError<DefaultDataCachePolicy>($"Type：{entityType.FullName} no primary key is set,unable to remove cache data");
                return;
            }

            //cache prefix keys
            var cachePrefixKeys = entityConfiguration.CachePrefixKeys ?? new List<string>(0);

            //cache keys
            var cacheKeys = entityConfiguration.CacheKeys ?? new List<string>(0);
            var cacheObject = new CacheObject()
            {
                ObjectName = objectName
            };
            List<CacheKey> cacheOptionKeys = new List<CacheKey>();
            foreach (var data in datas)
            {
                //prefix cache keys
                var dataPrefixKey = new CacheKey();
                if (!cachePrefixKeys.IsNullOrEmpty())
                {
                    foreach (var preKey in cachePrefixKeys)
                    {
                        var preKeyVal = data.GetValue(preKey)?.ToString() ?? string.Empty;
                        dataPrefixKey.AddName(preKey, preKeyVal);
                    }
                }

                //data cache key
                var dataCacheKey = new CacheKey(cacheObject, dataPrefixKey);
                foreach (string pk in primaryKeys)
                {
                    var pkValue = data.GetValue(pk);
                    dataCacheKey.AddName(pk, pkValue == null ? string.Empty : pkValue.ToString());
                }
                cacheOptionKeys.Add(dataCacheKey);

                //cache keys
                if (!cacheKeys.IsNullOrEmpty())
                {
                    foreach (string key in cacheKeys)
                    {
                        var otherCacheKey = new CacheKey(cacheObject, dataPrefixKey);
                        var cacheKeyValue = data.GetValue(key);
                        otherCacheKey.AddName(key, cacheKeyValue == null ? string.Empty : cacheKeyValue.ToString());
                        cacheOptionKeys.Add(otherCacheKey);
                    }
                }
            }
            if (cacheOptionKeys.IsNullOrEmpty())
            {
                return;
            }
            DeleteOptions removeCommand = new DeleteOptions()
            {
                CacheObject = cacheObject,
                Keys = cacheOptionKeys
            };
            var result = CacheManager.Keys.Delete(removeCommand);
            if (result != null && !result.Responses.IsNullOrEmpty())
            {
                foreach (var response in result.Responses)
                {
                    if (!string.IsNullOrWhiteSpace(response?.Message))
                    {
                        LogManager.LogInformation<DefaultDataCachePolicy>(response.Message);
                    }
                }
            }
        }

        #endregion

        #region RemoveCacheDataByType

        /// <summary>
        /// Remove cache data by type
        /// </summary>
        /// <param name="type">Data type</param>
        protected virtual void RemoveCacheDataByType(Type type)
        {
            if (type == null)
            {
                return;
            }
            var cacheObject = new CacheObject()
            {
                ObjectName = type.Name
            };
            CacheKey mateKey = new CacheKey(cacheObject);
            do
            {
                var getKeysCommand = new GetKeysOptions()
                {
                    CacheObject = cacheObject,
                    Query = new KeyQuery()
                    {
                        MateKey = mateKey.GetActualKey(),
                        Type = KeyMatchPattern.StartWith,
                        Page = 1,
                        PageSize = 10000
                    }
                };
                var keyResponses = CacheManager.Keys.GetKeysAsync(getKeysCommand).Result?.Responses;
                if (keyResponses.IsNullOrEmpty())
                {
                    break;
                }
                List<CacheKey> cacheKeys = new List<CacheKey>();
                foreach (var response in keyResponses)
                {
                    if (response?.Keys.IsNullOrEmpty() ?? true)
                    {
                        continue;
                    }
                    foreach (var key in response.Keys)
                    {
                        cacheKeys.Add(key);
                    }
                }
                if (cacheKeys.IsNullOrEmpty())
                {
                    break;
                }
                var keyDeleteCommand = new DeleteOptions()
                {
                    CacheObject = cacheObject,
                    Keys = cacheKeys
                };
                var result = CacheManager.Keys.Delete(keyDeleteCommand);
                if (result != null && !result.Responses.IsNullOrEmpty())
                {
                    foreach (var response in result.Responses)
                    {
                        if (!string.IsNullOrWhiteSpace(response?.Message))
                        {
                            LogManager.LogInformation<DefaultDataCachePolicy>(response.Message);
                        }
                    }
                }
            } while (true);
        }

        #endregion

        #region Get cache datas

        /// <summary>
        /// Get cache datas core
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <param name="size">Return data size</param>
        /// <returns></returns>
        QueryDataResult<T> GetCacheDatas<T>(IQuery query, int size)
        {
            //complex query force query database
            if (query?.IsComplexQuery ?? false)
            {
                return QueryDataResult<T>.Default("IQuery is a complex query object,not query cache data");
            }
            //query all data
            if (query?.NoneCondition ?? true)
            {
                List<T> datas = null;
                bool queryDatabase = true;
                if (size > 0)
                {
                    datas = GetCacheDatasByType<T>(query, query?.QuerySize ?? 1);
                    queryDatabase = datas?.Count < size;
                }
                return new QueryDataResult<T>()
                {
                    Datas = datas,
                    QueryDatabase = queryDatabase
                };
            }
            return GetCacheDatasByCondition<T>(query, size);
        }

        #endregion

        #region Get cache datas by type

        /// <summary>
        /// Get cache datas by type
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="size">Return data size</param>
        /// <returns></returns>
        protected virtual List<T> GetCacheDatasByType<T>(IQuery query, int size)
        {
            var type = typeof(T);
            var needSort = query != null && !query.Orders.IsNullOrEmpty();
            if (needSort)
            {
                return new List<T>(0);
            }

            #region get cache keys

            var typeName = type.Name;
            var primaryKeys = EntityManager.GetPrimaryKeys(type);
            var firstPrimaryKey = primaryKeys?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(firstPrimaryKey))
            {
                return new List<T>(0);
            }
            var cacheObject = new CacheObject()
            {
                ObjectName = typeName
            };
            CacheKey mateKey = new CacheKey(cacheObject);
            mateKey.AddName(firstPrimaryKey);
            var getKeysCommand = new GetKeysOptions()
            {
                CacheObject = cacheObject,
                Query = new KeyQuery()
                {
                    MateKey = mateKey.GetActualKey(),
                    Type = KeyMatchPattern.StartWith,
                    Page = 1,
                    PageSize = size > 0 ? size : int.MaxValue
                }
            };
            var keyResponses = CacheManager.Keys.GetKeysAsync(getKeysCommand).Result?.Responses;
            if (keyResponses.IsNullOrEmpty())
            {
                return new List<T>(0);
            }
            List<CacheKey> dataKeys = new List<CacheKey>();
            foreach (var response in keyResponses)
            {
                if (response?.Keys.IsNullOrEmpty() ?? true)
                {
                    continue;
                }
                bool fullData = false;
                foreach (var key in response.Keys)
                {
                    if (size > 0 && dataKeys.Count >= size)
                    {
                        fullData = true;
                        break;
                    }
                    dataKeys.Add(key);
                }
                if (fullData)
                {
                    break;
                }
            }
            if (dataKeys.IsNullOrEmpty())
            {
                return new List<T>(0);
            }

            #endregion

            #region get cache data

            return CacheManager.GetDataList<T>(dataKeys, cacheObject);

            #endregion
        }

        #endregion

        #region Get cache datas by condition

        /// <summary>
        /// Get cache datas by condition
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="query">Query condition</param>
        /// <param name="size">Return data size</param>
        /// <returns></returns>
        protected virtual QueryDataResult<T> GetCacheDatasByCondition<T>(IQuery query, int size)
        {
            Type entityType = typeof(T);
            var entityConfiguration = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfiguration == null)
            {
                LogManager.LogError<DefaultDataCachePolicy>($"Entity :{entityType.FullName} configuration is null");
                return QueryDataResult<T>.Default();
            }
            var primaryKeys = entityConfiguration.PrimaryKeys;
            if (primaryKeys.IsNullOrEmpty())
            {
                LogManager.LogError<T>($"Type：{entityType.FullName} no primary key is set,unable to get cache data");
                return new QueryDataResult<T>()
                {
                    Datas = new List<T>(0),
                    QueryDatabase = true
                };
            }
            var otherKeys = query.AllConditionFieldNames;
            var cacheObject = new CacheObject()
            {
                ObjectName = entityType.Name
            };

            #region cache prefix keys

            List<CacheKey> prefixDataKeys = new List<CacheKey>();
            var cachePrefixKeys = entityConfiguration.CachePrefixKeys ?? new List<string>(0);
            var prefixKeyValues = query.GetKeysEqualValue(cachePrefixKeys);
            if (prefixKeyValues.Count != cachePrefixKeys.Count)
            {
                LogManager.LogError<T>($"Type：{entityType.FullName} miss cache prefix key values in IQuery");
                return new QueryDataResult<T>()
                {
                    Datas = new List<T>(0),
                    QueryDatabase = true
                };
            }
            int preIndex = 0;
            foreach (var valItem in prefixKeyValues)
            {
                foreach (var prefixVal in valItem.Value)
                {
                    if (preIndex == 0)
                    {
                        var prefixDataKey = new CacheKey();
                        prefixDataKey.AddName(valItem.Key, prefixVal?.ToString() ?? string.Empty);
                        prefixDataKeys.Add(prefixDataKey);
                    }
                    else
                    {
                        foreach (var pdk in prefixDataKeys)
                        {
                            pdk.AddName(valItem.Key, prefixVal?.ToString() ?? string.Empty);
                        }
                    }
                }
                preIndex++;
            }
            otherKeys = otherKeys.Except(cachePrefixKeys);

            #endregion

            List<CacheKey> dataCacheKeys = new List<CacheKey>(otherKeys.Count());

            #region cache ignore keys

            var cacheIgnoreKeys = entityConfiguration.CacheIgnoreKeys ?? new List<string>(0);
            otherKeys = otherKeys.Except(cacheIgnoreKeys).ToList();

            #endregion

            #region primary keys

            var primaryKeyValues = query.GetKeysEqualValue(primaryKeys);
            bool fullPrimaryKey = primaryKeyValues.Count == primaryKeys.Count;
            if (fullPrimaryKey)
            {
                otherKeys = otherKeys.Except(primaryKeys).ToList();
                List<CacheKey> primaryCacheKeys = new List<CacheKey>();
                int pindex = 0;
                foreach (var valueItem in primaryKeyValues)
                {
                    if (valueItem.Value.IsNullOrEmpty())
                    {
                        continue;
                    }
                    foreach (var primaryVal in valueItem.Value)
                    {
                        if (pindex == 0)
                        {
                            var primaryCacheKey = new CacheKey(cacheObject, prefixDataKeys);
                            primaryCacheKey.AddName(valueItem.Key, primaryVal?.ToString() ?? string.Empty);
                            primaryCacheKeys.Add(primaryCacheKey);
                        }
                        else
                        {
                            foreach (var cacheKey in primaryCacheKeys)
                            {
                                cacheKey.AddName(valueItem.Key, primaryVal?.ToString() ?? string.Empty);
                            }
                        }
                    }
                    pindex++;
                }
                dataCacheKeys.AddRange(primaryCacheKeys);
            }

            #endregion

            #region cache fields

            var cacheFields = entityConfiguration.CacheKeys ?? new List<string>(0);
            List<CacheKey> cacheFieldCacheKeys = null;
            if (!cacheFields.IsNullOrEmpty())
            {
                var dataCacheFieldValues = query.GetKeysEqualValue(cacheFields);
                if (!dataCacheFieldValues.IsNullOrEmpty())
                {
                    otherKeys = otherKeys.Except(dataCacheFieldValues.Keys).ToList();
                    cacheFieldCacheKeys = new List<CacheKey>();
                    foreach (var valueItem in dataCacheFieldValues)
                    {
                        if (valueItem.Value.IsNullOrEmpty())
                        {
                            continue;
                        }
                        foreach (var val in valueItem.Value)
                        {
                            var cacheKey = new CacheKey(cacheObject, prefixDataKeys);
                            cacheKey.AddName(valueItem.Key, val?.ToString() ?? string.Empty);
                            cacheFieldCacheKeys.Add(cacheKey);
                        }
                    }
                    dataCacheKeys.AddRange(CacheManager.String.Get(cacheFieldCacheKeys, cacheObject).Select(c => ConstantCacheKey.Create(c)));
                }
            }

            #endregion

            bool needSort = query != null && !query.Orders.IsNullOrEmpty();
            if (dataCacheKeys.IsNullOrEmpty() || (!otherKeys.IsNullOrEmpty() && needSort))
            {
                LogManager.LogInformation<DefaultDataCachePolicy>($"Type：{entityType.FullName},IQuery has any other criterias without cache keys and need sort");
                return new QueryDataResult<T>()
                {
                    QueryDatabase = true,
                    Datas = new List<T>(0)
                };
            }
            dataCacheKeys = dataCacheKeys.Distinct().ToList();
            int removeCount = 0;
            var queryConditionFunc = query.GetQueryExpression<T>();//query condition
            List<T> dataList = new List<T>();
            size = size < 0 ? 0 : size;//return value count
            bool notQueryDb = false;
            CacheManager.GetDataList<T>(dataCacheKeys, cacheObject).ForEach(data =>
            {
                if (data != null && (queryConditionFunc?.Invoke(data) ?? true))
                {
                    dataList.Add(data);
                }
                else
                {
                    removeCount += data != null || DataCacheManager.Configuration.EnableCacheNullData ? 1 : 0;
                }
            });
            if (otherKeys.IsNullOrEmpty())
            {
                size = size <= 0 ? dataCacheKeys.Count : (size > dataCacheKeys.Count ? dataCacheKeys.Count : size);
                notQueryDb = dataList.Count >= size - removeCount;
                if (notQueryDb && needSort)
                {
                    dataList = query.Sort(dataList).ToList();
                }
            }
            else
            {
                notQueryDb = size > 0 && dataList.Count >= size;
            }
            return new QueryDataResult<T>()
            {
                QueryDatabase = !notQueryDb,
                QuriedCache = true,
                PrimaryCacheKeys = dataCacheKeys,
                OtherCacheKeys = cacheFieldCacheKeys,
                Datas = dataList
            };
        }

        #endregion  

        #endregion
    }
}
