using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess.Event;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.Paging;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// Data access base
    /// </summary>
    public abstract class BaseDataAccess<TEntity> : IDataAccess<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        internal static readonly Type entityType = typeof(TEntity);

        #region Implementation

        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return command</returns>
        public ICommand Add(TEntity data)
        {
            InitVersionFieldValue(data);//init version field value
            InitRefreshDateFieldValue(data);//init refresh date value
            var command = ExecuteAdd(data);

            //publish add event
            DataAccessEventBus.PublishAddEvent(data, data.GetAllValues());

            return command;
        }

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return command list</returns>
        public List<ICommand> Add(IEnumerable<TEntity> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<ICommand>(0);
            }
            List<ICommand> cmds = new List<ICommand>();
            foreach (var data in datas)
            {
                cmds.Add(Add(data));
            }
            return cmds;
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <returns>Return command</returns>
        public ICommand Modify(TEntity newData, TEntity oldData)
        {
            return Modify(newData, oldData, QueryManager.AppendEntityIdentityCondition(newData));
        }

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <param name="query">Query object</param>
        /// <returns>Return command</returns>
        public ICommand Modify(TEntity newData, TEntity oldData, IQuery query)
        {
            Dictionary<string, dynamic> modifyValues = newData.GetModifyValues(oldData);
            if (modifyValues == null || modifyValues.Count < 1)
            {
                return null;
            }
            if (query == null)
            {
                throw new EZNEWException("The data modification condition is null");
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(entityType);
            if (!string.IsNullOrWhiteSpace(versionFieldName))
            {
                var nowVersionValue = newData.GetValue(versionFieldName);
                if (!modifyValues.ContainsKey(versionFieldName))
                {
                    var newVersionValue = nowVersionValue + 1;
                    newData.SetValue(versionFieldName, newVersionValue);
                    modifyValues.Add(versionFieldName, newVersionValue);
                }
                query = AndExtensions.And(query, versionFieldName, CriteriaOperator.Equal, nowVersionValue);
            }

            #endregion

            #region refresh date

            string refreshFieldName = EntityManager.GetRefreshDateField(entityType);
            if (!string.IsNullOrWhiteSpace(refreshFieldName))
            {
                if (!modifyValues.ContainsKey(refreshFieldName))
                {
                    var nowDate = DateTimeOffset.Now;
                    newData.SetValue(refreshFieldName, nowDate);
                    modifyValues.Add(refreshFieldName, nowDate);
                }
            }

            #endregion

            var originValues = oldData.GetAllValues();
            var command = ExecuteModifyData(originValues, modifyValues, query);

            //publish modify data event
            DataAccessEventBus.PublishModifyDataEvent<TEntity>(originValues, modifyValues, query);

            return command;
        }

        /// <summary>
        /// edit data with expression
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">Query object</param>
        /// <returns>ICommand object</returns>
        public ICommand Modify(IModify modifyExpression, IQuery query)
        {
            Dictionary<string, IModifyValue> fieldAndValues = modifyExpression.GetModifyValues();
            if (fieldAndValues == null || fieldAndValues.Count <= 0)
            {
                return null;
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(versionFieldName))
            {
                if (!fieldAndValues.ContainsKey(versionFieldName))
                {
                    fieldAndValues.Add(versionFieldName, new CalculateModifyValue(CalculateOperator.Add, 1));
                }
            }

            #endregion

            #region update date

            string refreshFieldName = EntityManager.GetRefreshDateField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(refreshFieldName))
            {
                if (!fieldAndValues.ContainsKey(refreshFieldName))
                {
                    fieldAndValues.Add(refreshFieldName, new FixedModifyValue(DateTimeOffset.Now));
                }
            }

            #endregion

            var command = ExecuteModifyExpression(fieldAndValues, query);

            //publish modify expression event
            DataAccessEventBus.PublishModifyExpressionEvent<TEntity>(fieldAndValues, query);

            return command;
        }

        #endregion

        #region Delete data

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return command</returns>
        public ICommand Delete(TEntity data)
        {
            var command = ExecuteDeleteData(data);

            //publish delete data event
            DataAccessEventBus.PublishDeleteEvent(data, data.GetPrimaryKeyValues());

            return command;
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Return command</returns>
        public ICommand Delete(IQuery query)
        {
            var command = ExecuteDeleteByCondition(query);

            //publish delete by condition event
            DataAccessEventBus.PublishDeleteByCondition<TEntity>(query);

            return command;
        }

        #endregion

        #region Query data

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public TEntity Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public async Task<TEntity> GetAsync(IQuery query)
        {
            var data = await ExecuteGetAsync(query).ConfigureAwait(false);

            //publish query event
            DataAccessEventBus.PublishQueryEvent(query, new TEntity[1] { data });

            return data;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public List<TEntity> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public async Task<List<TEntity>> GetListAsync(IQuery query)
        {
            var dataList = await ExecuteGetListAsync(query).ConfigureAwait(false);

            //publish query event
            DataAccessEventBus.PublishQueryEvent(query, dataList);

            return dataList;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public IPaging<TEntity> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public async Task<IPaging<TEntity>> GetPagingAsync(IQuery query)
        {
            var dataPaging = await ExecuteGetPagingAsync(query).ConfigureAwait(false);

            //publish query event
            DataAccessEventBus.PublishQueryEvent(query, dataPaging);

            return dataPaging;
        }

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether does data is exist</returns>
        public bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether does data is exist</returns>
        public async Task<bool> ExistAsync(IQuery query)
        {
            var existValue = await ExecuteExistAsync(query).ConfigureAwait(false);

            //publish check data event
            DataAccessEventBus.PublishCheckDataEvent<TEntity>(query, existValue);

            return existValue;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public TValue Max<TValue>(IQuery query)
        {
            return MaxAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            var value = await ExecuteMaxAsync<TValue>(query).ConfigureAwait(false);

            //publish func event
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(OperateType.Max, value, query);

            return value;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public TValue Min<TValue>(IQuery query)
        {
            return MinAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            var value = await ExecuteMinAsync<TValue>(query).ConfigureAwait(false);

            //publish func event
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(OperateType.Min, value, query);

            return value;
        }

        /// <summary>
        /// Caculate sum
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the caculated value</returns>
        public TValue Sum<TValue>(IQuery query)
        {
            return SumAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Caculate sum
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the caculated value</returns>
        public async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            var value = await ExecuteSumAsync<TValue>(query).ConfigureAwait(false);

            //publish func event
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(OperateType.Sum, value, query);

            return value;
        }

        /// <summary>
        /// Caculate average
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        public TValue Avg<TValue>(IQuery query)
        {
            return AvgAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Caculate average
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        public async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            var value = await ExecuteAvgAsync<TValue>(query).ConfigureAwait(false);

            //publish func event
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(OperateType.Avg, value, query);

            return value;
        }

        /// <summary>
        /// Caculate count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// Caculate count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public async Task<long> CountAsync(IQuery query)
        {
            var value = await ExecuteCountAsync(query).ConfigureAwait(false);

            //publish func event
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(OperateType.Count, value, query);

            return value;
        }

        #endregion

        #endregion

        #region Function

        #region  Execute add

        /// <summary>
        /// Execute add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return add command</returns>
        protected abstract ICommand ExecuteAdd(TEntity data);

        #endregion

        #region  Execute modify

        /// <summary>
        /// Execute modify data
        /// </summary>
        /// <param name="originalValues">Original values</param>
        /// <param name="newValues">New values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify data command</returns>
        protected abstract ICommand ExecuteModifyData(Dictionary<string, dynamic> originalValues, Dictionary<string, dynamic> newValues, IQuery query);

        /// <summary>
        /// Execute modify expression
        /// </summary>
        /// <param name="modifyValues">Modify values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify expression command</returns>
        protected abstract ICommand ExecuteModifyExpression(Dictionary<string, IModifyValue> modifyValues, IQuery query);

        #endregion

        #region  Execute delete

        /// <summary>
        /// Execute delete data async
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return delete data command</returns>
        protected abstract ICommand ExecuteDeleteData(TEntity data);

        /// <summary>
        /// Execute delete by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return delete by condition command</returns>
        protected abstract ICommand ExecuteDeleteByCondition(IQuery query);

        #endregion

        #region  Execute query

        /// <summary>
        /// Execute get 
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        protected abstract Task<TEntity> ExecuteGetAsync(IQuery query);

        /// <summary>
        /// Execute get list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        protected abstract Task<List<TEntity>> ExecuteGetListAsync(IQuery query);

        /// <summary>
        /// Execute get paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        protected abstract Task<IPaging<TEntity>> ExecuteGetPagingAsync(IQuery query);

        /// <summary>
        /// Execute exists
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether does data is exist</returns>
        protected abstract Task<bool> ExecuteExistAsync(IQuery query);

        /// <summary>
        /// Execute get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected abstract Task<TValue> ExecuteMaxAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected abstract Task<TValue> ExecuteMinAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected abstract Task<TValue> ExecuteSumAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute get avg value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the avg value</returns>
        protected abstract Task<TValue> ExecuteAvgAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute get count value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the data count</returns>
        protected abstract Task<long> ExecuteCountAsync(IQuery query);

        #endregion

        #endregion

        #region Util

        #region Init data value

        /// <summary>
        /// Init version value
        /// </summary>
        /// <param name="data">Entity data</param>
        protected void InitVersionFieldValue(TEntity data)
        {
            if (data == null)
            {
                return;
            }
            string versionField = EntityManager.GetVersionField(typeof(TEntity));
            if (string.IsNullOrWhiteSpace(versionField))
            {
                return;
            }
            long initValue = 1;
            data.SetValue(versionField, initValue);
        }

        /// <summary>
        /// Init refresh date
        /// </summary>
        /// <param name="data">Entity data</param>
        protected void InitRefreshDateFieldValue(TEntity data)
        {
            if (data == null)
            {
                return;
            }
            string refreshDateField = EntityManager.GetRefreshDateField(typeof(TEntity));
            if (string.IsNullOrWhiteSpace(refreshDateField))
            {
                return;
            }
            data.SetValue(refreshDateField, DateTimeOffset.Now);
        }

        #endregion

        #endregion
    }
}
