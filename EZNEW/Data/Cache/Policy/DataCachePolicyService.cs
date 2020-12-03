using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EZNEW.Data.Cache.Command;
using EZNEW.DependencyInjection;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Logging;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Data cache policy service
    /// </summary>
    internal class DataCachePolicyService
    {
        private static readonly IDataCachePolicy dataCachePolicy;

        static DataCachePolicyService()
        {
            dataCachePolicy = ContainerManager.Resolve<IDataCachePolicy>();
            if (dataCachePolicy == null)
            {
                dataCachePolicy = new DefaultDataCachePolicy();
            }
        }

        #region Add data

        /// <summary>
        /// Add data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Data cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void AddData<T>(AddDataCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            if (cacheCommand == null || databaseCommand == null)
            {
                return;
            }
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.Add);
            if (cacheOperationConfig == null)
            {
                return;
            }
            //database command starting event
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, new CommandStartingEventParameter()
                {
                    CommandBehavior = CommandBehavior.Add,
                    Command = databaseCommand,
                    Data = cacheCommand.Data
                }, !cacheOperationConfig.Synchronous);
            }

            //save data to cache after command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, new CommandCallbackEventParameter()
                {
                    CommandBehavior = CommandBehavior.Add,
                    Command = databaseCommand,
                    Data = cacheCommand.Data
                });
            }
        }

        #endregion

        #region Modify data

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void Modify<T>(ModifyDataCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.ModifyData);
            if (cacheOperationConfig == null)
            {
                return;
            }
            //remove data to cache before command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, new CommandStartingEventParameter()
                {
                    CommandBehavior = CommandBehavior.Update,
                    Command = databaseCommand,
                    Data = cacheCommand.OldData
                }, !cacheOperationConfig.Synchronous);
            }

            //remove data to cache after command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, new CommandCallbackEventParameter()
                {
                    CommandBehavior = CommandBehavior.Update,
                    Data = cacheCommand.OldData,
                    Command = databaseCommand
                });
            }
        }

        #endregion

        #region Modify data by condition

        /// <summary>
        /// Modify data by condition
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void ModifyByCondition<T>(ModifyByConditionCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            var query = cacheCommand.Query;
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.ModifyByCondition);
            if (cacheOperationConfig == null)
            {
                return;
            }
            CommandCallbackEventParameter callbackParameter = null;
            CommandStartingEventParameter beforeParameter = null;
            if (query?.NoneCondition ?? true)
            {
                callbackParameter = new CommandCallbackEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.UpdateObjectType,
                    Data = typeof(T)
                };
                beforeParameter = new CommandStartingEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.RemoveObjectType,
                    Data = typeof(T)
                };
            }
            else if (cacheCommand.GetDataListProxy != null)
            {
                Tuple<Func<IQuery, List<T>>, IQuery> updateOption = new Tuple<Func<IQuery, List<T>>, IQuery>(cacheCommand.GetDataListProxy, cacheCommand.Query);

                #region update cache before execute command

                beforeParameter = new CommandStartingEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.UpdateByQuery,
                    Data = updateOption
                };

                #endregion

                #region update cache after execute command

                callbackParameter = new CommandCallbackEventParameter()
                {
                    CommandBehavior = CommandBehavior.UpdateByQuery,
                    Data = updateOption,
                    Command = databaseCommand
                };

                #endregion
            }
            //remove data to cache before command execute
            if (beforeParameter != null && (cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, beforeParameter, !cacheOperationConfig.Synchronous);
            }

            //update data to cache after command execute
            if (callbackParameter != null && (cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, callbackParameter);
            }
        }

        /// <summary>
        /// Modify data by condition
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void ModifyDataByCondition<T>(ModifyDataByConditionCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            var query = cacheCommand.Query;
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.ModifyByCondition);
            if (cacheOperationConfig == null)
            {
                return;
            }
            CommandCallbackEventParameter callbackParameter = null;
            CommandStartingEventParameter beforeParameter = null;
            if (query?.NoneCondition ?? true)
            {
                callbackParameter = new CommandCallbackEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.UpdateObjectType,
                    Data = typeof(T)
                };
                beforeParameter = new CommandStartingEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.RemoveObjectType,
                    Data = typeof(T)
                };
            }
            else if (cacheCommand.GetDataListProxy != null)
            {
                Tuple<Func<IQuery, List<T>>, IQuery> updateOption = new Tuple<Func<IQuery, List<T>>, IQuery>(cacheCommand.GetDataListProxy, cacheCommand.Query);

                #region update cache before execute command

                beforeParameter = new CommandStartingEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.UpdateByQuery,
                    Data = updateOption
                };

                #endregion

                #region update cache after execute command

                callbackParameter = new CommandCallbackEventParameter()
                {
                    CommandBehavior = CommandBehavior.UpdateByQuery,
                    Data = updateOption,
                    Command = databaseCommand
                };

                #endregion
            }
            //remove data to cache before command execute
            if (beforeParameter != null && (cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, beforeParameter, !cacheOperationConfig.Synchronous);
            }

            //remove data to cache after command execute
            if (callbackParameter != null && (cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, callbackParameter);
            }
        }

        #endregion

        #region Remove data

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void Remove<T>(RemoveDataCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.RemoveData);
            if (cacheOperationConfig == null)
            {
                return;
            }
            //remove data to cache before command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, new CommandStartingEventParameter()
                {
                    CommandBehavior = CommandBehavior.RemoveData,
                    Command = databaseCommand,
                    Data = cacheCommand.Data
                }, !cacheOperationConfig.Synchronous);
            }

            //remove data to cache after command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, new CommandCallbackEventParameter()
                {
                    CommandBehavior = CommandBehavior.RemoveData,
                    Data = cacheCommand.Data,
                    Command = databaseCommand
                });
            }
        }

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Cache command</param>
        /// <param name="databaseCommand">Database command</param>
        public void RemoveByCondition<T>(RemoveByConditionCacheCommand<T> cacheCommand, ICommand databaseCommand) where T : BaseEntity<T>, new()
        {
            CommandCallbackEventParameter callbackParameter = null;
            CommandStartingEventParameter beforeParameter = null;
            var cacheOperationConfig = DataCacheManager.Configuration.GetDataCacheOperationConfiguration(DataOperationType.RemoveByCondition);
            if (cacheOperationConfig == null)
            {
                return;
            }
            if (cacheCommand.Query?.NoneCondition ?? true)
            {
                callbackParameter = new CommandCallbackEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.RemoveObjectType,
                    Data = typeof(T)
                };
                beforeParameter = new CommandStartingEventParameter()
                {
                    Command = databaseCommand,
                    CommandBehavior = CommandBehavior.RemoveObjectType,
                    Data = typeof(T)
                };
            }
            else if (cacheCommand.GetDataListProxy != null)
            {
                List<T> dataList = GetDatabaseDataList(cacheCommand.Query, cacheCommand.GetDataListProxy);
                if (!dataList.IsNullOrEmpty())
                {
                    callbackParameter = new CommandCallbackEventParameter()
                    {
                        Command = databaseCommand,
                        CommandBehavior = CommandBehavior.RemoveByQuery,
                        Data = new Tuple<IEnumerable<T>, IQuery>(dataList, cacheCommand.Query)
                    };
                    beforeParameter = new CommandStartingEventParameter()
                    {
                        Data = new Tuple<IEnumerable<T>, IQuery>(dataList, cacheCommand.Query),
                        Command = databaseCommand,
                        CommandBehavior = CommandBehavior.RemoveByQuery
                    };
                }
            }
            //remove data to cache before command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.Before) != 0)
            {
                databaseCommand.ListenStarting(CommandStartingEventHandler<T>, beforeParameter, !cacheOperationConfig.Synchronous);
            }

            //remove data to cache after command execute
            if ((cacheOperationConfig.TriggerTime & DataCacheOperationTriggerTime.After) != 0)
            {
                databaseCommand.ListenCallback(CommandCallbackEventHandler<T>, callbackParameter);
            }
        }

        #endregion

        #region Query data

        /// <summary>
        /// Query data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return query result</returns>
        public QueryDataResult<T> Query<T>(IQuery query) where T : BaseEntity<T>, new()
        {
            return dataCachePolicy.OnQueryStarting(new QueryDataContext<T>()
            {
                Query = query
            });
        }

        #endregion

        #region Query callback

        /// <summary>
        /// Query callback
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="queryDataCallbackContext">Query data callback context</param>
        public void QueryCallback<T>(QueryDataCallbackContext<T> queryDataCallbackContext) where T : BaseEntity<T>, new()
        {
            dataCachePolicy.OnQueryCallback(queryDataCallbackContext);
        }

        #endregion

        #region Command starting event handler

        /// <summary>
        /// Command starting event handler
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="parameter">Command starting event parameter</param>
        /// <returns>Return command starting event execute result</returns>
        CommandStartingEventExecuteResult CommandStartingEventHandler<T>(CommandStartingEventParameter parameter) where T : BaseEntity<T>, new()
        {
            if (parameter == null || parameter.Command == null)
            {
                return CommandStartingEventExecuteResult.DefaultSuccess;
            }
            StartingResult startingResult = null;
            switch (parameter.CommandBehavior)
            {
                case CommandBehavior.Add:
                    startingResult = dataCachePolicy.OnAddStarting(new AddDataContext<T>() { Datas = new List<T>(1) { parameter.Data as T }, DatabaseCommand = parameter.Command });
                    break;
                case CommandBehavior.RemoveByQuery:
                    Tuple<IEnumerable<T>, IQuery> removeOption = parameter.Data as Tuple<IEnumerable<T>, IQuery>;
                    startingResult = dataCachePolicy.OnRemoveByQueryStarting(new RemoveByQueryContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = removeOption?.Item1,
                        Query = removeOption?.Item2
                    });
                    break;
                case CommandBehavior.UpdateByQuery:
                    Tuple<Func<IQuery, List<T>>, IQuery> updateOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    startingResult = dataCachePolicy.OnUpdateByQueryStarting(new UpdateByQueryContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        GetDatasProxy = updateOption?.Item1,
                        Query = updateOption?.Item2
                    });
                    break;
                case CommandBehavior.RemoveObjectType:
                    Tuple<Func<IQuery, List<T>>, IQuery> removeAllOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    startingResult = dataCachePolicy.OnRemoveAllStarting(new RemoveAllContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Query = removeAllOption?.Item2,
                        GetDatasProxy = removeAllOption?.Item1
                    });
                    break;
                case CommandBehavior.UpdateObjectType:
                    Tuple<Func<IQuery, List<T>>, IQuery> updateAllOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    startingResult = dataCachePolicy.OnUpdateAllStarting(new UpdateAllContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Query = updateAllOption?.Item2,
                        GetDatasProxy = updateAllOption?.Item1
                    });
                    break;
                case CommandBehavior.RemoveData:
                    startingResult = dataCachePolicy.OnRemoveStarting(new RemoveDataContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = new List<T>(1) { parameter.Data as T }
                    });
                    break;
                case CommandBehavior.Update:
                    startingResult = dataCachePolicy.OnUpdateStarting(new UpdateDataContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = new List<T>(1) { parameter.Data as T }
                    });
                    break;
                default:
                    break;
            }
            CommandStartingEventExecuteResult result = new CommandStartingEventExecuteResult
            {
                BreakCommand = startingResult?.BreakDatabaseCommand ?? false,
                Message = startingResult?.Message ?? string.Empty
            };
            return result;
        }

        #endregion

        #region Command callback event handler

        /// <summary>
        /// command cache call back
        /// </summary>
        /// <param name="parameter">command</param>
        /// <returns></returns>
        CommandCallbackEventExecuteResult CommandCallbackEventHandler<T>(CommandCallbackEventParameter parameter) where T : BaseEntity<T>, new()
        {
            if (parameter?.Command == null)
            {
                return CommandCallbackEventExecuteResult.Default;
            }
            switch (parameter.CommandBehavior)
            {
                case CommandBehavior.Add:
                    dataCachePolicy.OnAddCallback(new AddDataContext<T>()
                    {
                        Datas = new List<T>(1) { parameter.Data as T },
                        DatabaseCommand = parameter.Command
                    });
                    break;
                case CommandBehavior.RemoveByQuery:
                    Tuple<IEnumerable<T>, IQuery> removeOption = parameter.Data as Tuple<IEnumerable<T>, IQuery>;
                    dataCachePolicy.OnRemoveByQueryCallback(new RemoveByQueryContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = removeOption?.Item1,
                        Query = removeOption?.Item2
                    });
                    break;
                case CommandBehavior.UpdateByQuery:
                    Tuple<Func<IQuery, List<T>>, IQuery> updateOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    dataCachePolicy.OnUpdateByQueryCallback(new UpdateByQueryContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        GetDatasProxy = updateOption?.Item1,
                        Query = updateOption?.Item2
                    });
                    break;
                case CommandBehavior.RemoveObjectType:
                    Tuple<Func<IQuery, List<T>>, IQuery> removeAllOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    dataCachePolicy.OnRemoveAllCallback(new RemoveAllContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Query = removeAllOption?.Item2,
                        GetDatasProxy = removeAllOption?.Item1
                    });
                    break;
                case CommandBehavior.UpdateObjectType:
                    Tuple<Func<IQuery, List<T>>, IQuery> updateAllOption = parameter.Data as Tuple<Func<IQuery, List<T>>, IQuery>;
                    dataCachePolicy.OnUpdateAllCallback(new UpdateAllContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Query = updateAllOption?.Item2,
                        GetDatasProxy = updateAllOption?.Item1
                    });
                    break;
                case CommandBehavior.RemoveData:
                    dataCachePolicy.OnRemoveCallback(new RemoveDataContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = new List<T>(1) { parameter.Data as T }
                    });
                    break;
                case CommandBehavior.Update:
                    dataCachePolicy.OnUpdateCallback(new UpdateDataContext<T>()
                    {
                        DatabaseCommand = parameter.Command,
                        Datas = new List<T>(1) { parameter.Data as T }
                    });
                    break;
            }
            return CommandCallbackEventExecuteResult.Default;
        }

        #endregion

        #region Get database datas

        internal List<T> GetDatabaseDataList<T>(IQuery query, Func<IQuery, List<T>> getDatabaseDataListProxy)
        {
            if (query == null || getDatabaseDataListProxy == null)
            {
                return new List<T>(0);
            }
            var entityConfiguration = EntityManager.GetEntityConfiguration<T>();
            if (entityConfiguration == null)
            {
                LogManager.LogError<DataCachePolicyService>($"Entity : {typeof(T).FullName} configuration is null");
                return new List<T>(0);
            }
            var copyQuery = query.LightClone();
            copyQuery.ClearQueryFields();
            copyQuery.ClearNotQueryFields();
            var primaryKeys = entityConfiguration.PrimaryKeys;
            var cacheKeys = entityConfiguration.CacheKeys;
            copyQuery.AddQueryFields(primaryKeys?.ToArray());
            copyQuery.AddQueryFields(cacheKeys?.ToArray());
            return getDatabaseDataListProxy(copyQuery);
        }

        #endregion
    }
}
