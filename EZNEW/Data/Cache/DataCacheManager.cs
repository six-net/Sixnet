using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using EZNEW.Develop.Entity;
using EZNEW.Paging;
using EZNEW.DependencyInjection;
using EZNEW.Develop.Command;
using EZNEW.Data.Cache.Command;
using EZNEW.Queue;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Data cache manager
    /// </summary>
    public static class DataCacheManager
    {
        #region Configuration

        public static class Configuration
        {
            /// <summary>
            /// Data cache operation configuration
            /// </summary>
            static Dictionary<DataOperationType, DataCacheOperationConfiguration> CacheOperationConfigurations;

            /// <summary>
            /// Null data expiration
            /// </summary>
            public static TimeSpan? NullDataExpiration = TimeSpan.FromSeconds(30);

            /// <summary>
            /// Data expiration
            /// </summary>
            public static TimeSpan? DataExpiration = null;

            /// <summary>
            /// Max expiration float seconds
            /// Append a random second value between 0-maxValue when add cache data
            /// </summary>
            public static int MaxDataExpirationFloatSeconds = 60;

            /// <summary>
            /// Whether cache null data
            /// </summary>
            public static bool EnableCacheNullData = true;

            /// <summary>
            /// Null data cache key
            /// </summary>
            public static string NullDataCacheKey = "EZNEW_ENTITY_NULL_PRIMARYKEY";

            /// <summary>
            /// Data type null data expirations
            /// </summary>
            static readonly Dictionary<Guid, TimeSpan?> DataTypeNullDataExpirations = new Dictionary<Guid, TimeSpan?>();

            /// <summary>
            /// Data type expirations
            /// </summary>
            static readonly Dictionary<Guid, TimeSpan?> DataTypeExpirations = new Dictionary<Guid, TimeSpan?>();

            /// <summary>
            /// Default data cache behavior
            /// </summary>
            static readonly DataCacheBehavior DefaultDataCacheBehavior = new DataCacheBehavior() { ExceptionHandling = DataCacheExceptionHandling.Continue };

            /// <summary>
            /// Default data cache behavior proxy
            /// </summary>
            static readonly Func<DataCacheBehaviorContext, DataCacheBehavior> DefaultDataCacheBehaviorProxy = context =>
            {
                return DefaultDataCacheBehavior;
            };

            /// <summary>
            /// Data cache behavior proxy
            /// </summary>
            static Func<DataCacheBehaviorContext, DataCacheBehavior> DataCacheBehaviorProxy = null;

            static Configuration()
            {
                CacheOperationConfigurations = new Dictionary<DataOperationType, DataCacheOperationConfiguration>()
                {
                    {
                        DataOperationType.Add,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.After
                        }
                    },
                    {
                        DataOperationType.ModifyData,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.After
                        }
                    },
                    {
                        DataOperationType.ModifyByCondition,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.After
                        }
                    },
                    {
                        DataOperationType.Get,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.After
                        }
                    },
                    {
                        DataOperationType.RemoveData,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.Before|DataCacheOperationTriggerTime.After
                        }
                    },
                    {
                        DataOperationType.RemoveByCondition,new DataCacheOperationConfiguration()
                        {
                            TriggerTime=DataCacheOperationTriggerTime.Before|DataCacheOperationTriggerTime.After
                        }
                    },
                };
            }

            #region Operation configuration

            #region Configure data cache operation

            /// <summary>
            /// config cache operation
            /// </summary>
            /// <param name="dataOperationType">Data operation type</param>
            /// <param name="cacheOperationConfiguration">Cache operation configuration</param>
            public static void ConfigureDataCacheOperation(DataOperationType dataOperationType, DataCacheOperationConfiguration cacheOperationConfiguration)
            {
                if (cacheOperationConfiguration == null)
                {
                    return;
                }
                if (CacheOperationConfigurations == null)
                {
                    CacheOperationConfigurations = new Dictionary<DataOperationType, DataCacheOperationConfiguration>();
                }
                CacheOperationConfigurations[dataOperationType] = cacheOperationConfiguration;
            }

            #endregion

            #region Remove cache operation

            /// <summary>
            /// Remove data cache operation
            /// </summary>
            /// <param name="dataOperationTypes">Data operation types</param>
            public static void RemoveDataCacheOperation(params DataOperationType[] dataOperationTypes)
            {
                if (dataOperationTypes.IsNullOrEmpty())
                {
                    return;
                }
                foreach (var operation in dataOperationTypes)
                {
                    CacheOperationConfigurations?.Remove(operation);
                }
            }

            #endregion

            #region Remove all cache operation

            /// <summary>
            /// Remove all data cache operation
            /// </summary>
            public static void RemoveAllDataCacheOperation()
            {
                CacheOperationConfigurations?.Clear();
            }

            #endregion

            #region Get cache operation

            /// <summary>
            /// Get data operation cache configuration
            /// </summary>
            /// <param name="dataOperationType">Data operation type</param>
            /// <returns>Return Data cache operation configuration</returns>
            public static DataCacheOperationConfiguration GetDataCacheOperationConfiguration(DataOperationType dataOperationType)
            {
                DataCacheOperationConfiguration config = null;
                CacheOperationConfigurations?.TryGetValue(dataOperationType, out config);
                return config;
            }

            #endregion 

            #endregion

            #region Null data expiration

            /// <summary>
            /// Configure null data expiration
            /// </summary>
            /// <param name="dataType">Entity type</param>
            /// <param name="expiration">Expiration</param>
            public static void ConfigureNullDataExpiration(Type dataType, TimeSpan? expiration)
            {
                if (dataType == null)
                {
                    return;
                }
                DataTypeNullDataExpirations[dataType.GUID] = expiration;
            }

            /// <summary>
            /// Get null data expiration
            /// </summary>
            /// <param name="dataType">Data type</param>
            /// <returns>Return expiration</returns>
            public static TimeSpan? GetNullDataExpiration(Type dataType)
            {
                if (dataType == null || !DataTypeNullDataExpirations.ContainsKey(dataType.GUID))
                {
                    return NullDataExpiration;
                }
                return DataTypeNullDataExpirations[dataType.GUID];
            }

            #endregion

            #region Data expiration

            /// <summary>
            /// Data expiration
            /// </summary>
            /// <param name="dataType">Data type</param>
            /// <param name="expiration">Expiration</param>
            public static void ConfigureExpiration(Type dataType, TimeSpan? expiration)
            {
                if (dataType == null)
                {
                    return;
                }
                DataTypeExpirations[dataType.GUID] = expiration;
            }

            /// <summary>
            /// Get expiration
            /// </summary>
            /// <param name="dataType">Data type</param>
            /// <returns>Return expiration</returns>
            public static TimeSpan? GetExpiration(Type dataType)
            {
                if (dataType != null && DataTypeExpirations.ContainsKey(dataType.GUID))
                {
                    return DataTypeExpirations[dataType.GUID];
                }
                return DataExpiration;
            }

            #endregion

            #region Exception behavior

            /// <summary>
            /// Configure data cache behavior
            /// </summary>
            /// <param name="getDataCacheBehaviorProxy">Get data cache behavior proxy</param>
            public static void ConfigureBehavior(Func<DataCacheBehaviorContext, DataCacheBehavior> getDataCacheBehaviorProxy)
            {
                DataCacheBehaviorProxy = getDataCacheBehaviorProxy;
            }

            /// <summary>
            /// Get data cache behavior
            /// </summary>
            /// <param name="dataCacheBehaviorContext">Data cache behavior context</param>
            /// <returns>Return data cache behavior</returns>
            internal static DataCacheBehavior GetBehavior(DataCacheBehaviorContext dataCacheBehaviorContext)
            {
                var proxy = DataCacheBehaviorProxy ?? DefaultDataCacheBehaviorProxy;
                return proxy(dataCacheBehaviorContext) ?? DefaultDataCacheBehavior;
            }

            #endregion
        }

        #endregion
    }
}
