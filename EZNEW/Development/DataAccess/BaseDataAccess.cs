using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.Command;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess.Event;
using EZNEW.Development.Entity;
using EZNEW.Paging;
using EZNEW.Data;
using System.Linq;

namespace EZNEW.Development.DataAccess
{
    /// <summary>
    /// Defines base data access
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
            InitDefaultDateTimeFieldValue(data);//init datetime value
            var command = ExecuteAdding(data);

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
            var modifyQuery = QueryManager.AppendEntityIdentityCondition(newData).ExcludeObsoleteData();
            return Modify(newData, oldData, modifyQuery);
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
            if (query == null)
            {
                throw new ArgumentNullException($"{nameof(query)}");
            }

            Dictionary<string, dynamic> modificationValues = newData.GetModifiedValues(oldData);
            if (modificationValues.IsNullOrEmpty())
            {
                return null;
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(entityType);
            if (!string.IsNullOrWhiteSpace(versionFieldName) && !modificationValues.ContainsKey(versionFieldName))
            {
                var nowVersionValue = oldData.GetValue(versionFieldName);
                var newVersionValue = nowVersionValue + 1;
                newData.SetValue(versionFieldName, newVersionValue);
                modificationValues.Add(versionFieldName, newVersionValue);
                query = ConnectionExtensions.And(query, versionFieldName, CriterionOperator.Equal, nowVersionValue);
            }

            #endregion

            #region update time

            string updateDateTimeFieldName = EntityManager.GetUpdateTimeField(entityType);
            if (!string.IsNullOrWhiteSpace(updateDateTimeFieldName) && !modificationValues.ContainsKey(updateDateTimeFieldName))
            {
                var nowDate = DateTimeOffset.Now;
                newData.SetValue(updateDateTimeFieldName, nowDate);
                modificationValues.Add(updateDateTimeFieldName, nowDate);
            }

            #endregion

            var originValues = oldData.GetAllValues();
            var command = ExecuteDataModification(originValues, modificationValues, query);

            //publish modify data event
            DataAccessEventBus.PublishModifyDataEvent<TEntity>(originValues, modificationValues, query);

            return command;
        }

        /// <summary>
        /// Modify data by expression
        /// </summary>
        /// <param name="modification">Modification</param>
        /// <param name="query">Query object</param>
        /// <returns>ICommand object</returns>
        public ICommand Modify(IModification modification, IQuery query)
        {
            var modificationValues = modification?.ModificationEntries?.ToDictionary(c => c.FieldName, c => c.Value);
            if (modificationValues.IsNullOrEmpty())
            {
                return null;
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(versionFieldName) && !modificationValues.ContainsKey(versionFieldName))
            {
                modificationValues.Add(versionFieldName, new CalculationModificationValue(CalculationOperator.Add, 1));
            }

            #endregion

            #region update time

            string updateFieldName = EntityManager.GetUpdateTimeField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(updateFieldName) && !modificationValues.ContainsKey(updateFieldName))
            {
                modificationValues.Add(updateFieldName, new FixedModificationValue(DateTimeOffset.Now));
            }

            #endregion

            var command = ExecuteModification(modificationValues, query);

            //publish modify expression event
            DataAccessEventBus.PublishModifyExpressionEvent<TEntity>(modificationValues, query);

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
            //Execute delete
            var command = ExecuteDataDeletion(data);

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
            var command = ExecuteDeletion(query);

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
        public PagingInfo<TEntity> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public async Task<PagingInfo<TEntity>> GetPagingAsync(IQuery query)
        {
            var dataPaging = await ExecuteGetPagingAsync(query).ConfigureAwait(false);

            //publish query event
            DataAccessEventBus.PublishQueryEvent(query, dataPaging?.Items);

            return dataPaging;
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        public bool Exists(IQuery query)
        {
            return ExistsAsync(query).Result;
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether exists data</returns>
        public async Task<bool> ExistsAsync(IQuery query)
        {
            var existValue = await ExecuteExistsAsync(query).ConfigureAwait(false);

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
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(CommandOperationType.Max, value, query);

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
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(CommandOperationType.Min, value, query);

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
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(CommandOperationType.Sum, value, query);

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
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(CommandOperationType.Avg, value, query);

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
            DataAccessEventBus.PublishAggregateFunctionEvent<TEntity>(CommandOperationType.Count, value, query);

            return value;
        }

        #endregion

        #endregion

        #region Function

        #region  Execute adding

        /// <summary>
        /// Execute add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return add command</returns>
        protected abstract ICommand ExecuteAdding(TEntity data);

        #endregion

        #region  Execute modification

        /// <summary>
        /// Execute data modification
        /// </summary>
        /// <param name="originalValues">Original values</param>
        /// <param name="newValues">New values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify data command</returns>
        protected abstract ICommand ExecuteDataModification(Dictionary<string, dynamic> originalValues, Dictionary<string, dynamic> newValues, IQuery query);

        /// <summary>
        /// Execute modification
        /// </summary>
        /// <param name="modificationValues">Modification values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify expression command</returns>
        protected abstract ICommand ExecuteModification(Dictionary<string, IModificationValue> modificationValues, IQuery query);

        #endregion

        #region  Execute deletion

        /// <summary>
        /// Execute data deletion
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return a data deletion command</returns>
        protected abstract ICommand ExecuteDataDeletion(TEntity data);

        /// <summary>
        /// Execute deletion
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return a deletion command</returns>
        protected abstract ICommand ExecuteDeletion(IQuery query);

        #endregion

        #region  Execute query

        /// <summary>
        /// Execute query 
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
        protected abstract Task<PagingInfo<TEntity>> ExecuteGetPagingAsync(IQuery query);

        /// <summary>
        /// Execute exists
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether does data is exist</returns>
        protected abstract Task<bool> ExecuteExistsAsync(IQuery query);

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
        /// Init default datetime
        /// </summary>
        /// <param name="data">Entity data</param>
        protected void InitDefaultDateTimeFieldValue(TEntity data)
        {
            if (data == null)
            {
                return;
            }

            DateTimeOffset nowDate = DateTimeOffset.Now;

            //Creation datetime
            string creationDateTimeField = EntityManager.GetCreationTimeField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(creationDateTimeField))
            {
                data.SetValue(creationDateTimeField, nowDate);
            }

            //update datetime
            string updateDateTimeField = EntityManager.GetUpdateTimeField(typeof(TEntity));
            if (!string.IsNullOrWhiteSpace(updateDateTimeField))
            {
                data.SetValue(updateDateTimeField, nowDate);
            }
        }

        #endregion

        #endregion
    }
}
