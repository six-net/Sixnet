using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Develop.Entity;
using EZNEW.Develop.CQuery;
using EZNEW.Paging;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// Imeplements data access for rdb
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public abstract class RdbDataAccess<TEntity> : BaseDataAccess<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        #region Function

        #region  Execute add

        /// <summary>
        /// Execute add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return add command</returns>
        protected override ICommand ExecuteAdd(TEntity data)
        {
            var cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Insert, data.GetCommandParameters());
            SetCommand(cmd, data.GetAllValues());
            cmd.MustReturnValueOnSuccess = true;
            return cmd;
        }

        #endregion

        #region Execute Modify

        /// <summary>
        /// Execute modify data
        /// </summary>
        /// <param name="originalValues">Original values</param>
        /// <param name="newValues">New values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify command</returns>
        protected override ICommand ExecuteModifyData(Dictionary<string, dynamic> originalValues, Dictionary<string, dynamic> newValues, IQuery query)
        {
            return Update(newValues.Keys, newValues, query);
        }

        /// <summary>
        /// Execute modify expression
        /// </summary>
        /// <param name="modifyValues">Modify values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify command</returns>
        protected override ICommand ExecuteModifyExpression(Dictionary<string, IModifyValue> modifyValues, IQuery query)
        {
            return Update(modifyValues.Keys.ToList(), modifyValues, query);
        }

        /// <summary>
        /// Modify value
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return modify command</returns>
        ICommand Update(IEnumerable<string> fields, object parameters, IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Update);
            SetCommand(cmd, parameters as Dictionary<string, dynamic>);
            cmd.Fields = fields;
            cmd.Parameters = parameters;
            cmd.MustReturnValueOnSuccess = query?.MustReturnValueOnSuccess ?? false;
            cmd.Query = query;
            return cmd;
        }

        #endregion

        #region Execute delete

        /// <summary>
        /// Execute delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return delete command</returns>
        protected override ICommand ExecuteDeleteData(TEntity data)
        {
            return ExecuteDeleteByCondition(QueryManager.AppendEntityIdentityCondition(data));
        }

        /// <summary>
        /// Execute delete by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return delete command</returns>
        protected override ICommand ExecuteDeleteByCondition(IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Delete);
            SetCommand(cmd, null);
            cmd.MustReturnValueOnSuccess = query?.MustReturnValueOnSuccess ?? false;
            cmd.Query = query;
            return cmd;
        }

        #endregion

        #region Execute query

        /// <summary>
        /// Execute query data 
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        protected override async Task<TEntity> ExecuteGetAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            TEntity data = await WorkManager.QuerySingleAsync<TEntity>(cmd).ConfigureAwait(false);
            return data;
        }

        /// <summary>
        /// Execute query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        protected override async Task<List<TEntity>> ExecuteGetListAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            var dataList = (await WorkManager.QueryAsync<TEntity>(cmd).ConfigureAwait(false)).ToList();
            return dataList;
        }

        /// <summary>
        /// Execute query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        protected override async Task<IPaging<TEntity>> ExecuteGetPagingAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            var dataPaging = await WorkManager.QueryPagingAsync<TEntity>(cmd).ConfigureAwait(false);
            return dataPaging;
        }

        /// <summary>
        /// Execute exists
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        protected override async Task<bool> ExecuteExistAsync(IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<TEntity>(OperateType.Exist);
            SetCommand(cmd, null);
            cmd.MustReturnValueOnSuccess = true;
            cmd.Query = query;
            cmd.CommandResultType = ExecuteCommandResult.ExecuteScalar;
            return await WorkManager.QueryAsync(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected override async Task<TValue> ExecuteMaxAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(OperateType.Max, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected override async Task<TValue> ExecuteMinAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(OperateType.Min, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected override async Task<TValue> ExecuteSumAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(OperateType.Sum, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get avg value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the avg value</returns>
        protected override async Task<TValue> ExecuteAvgAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(OperateType.Avg, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get count value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        protected override async Task<long> ExecuteCountAsync(IQuery query)
        {
            return await AggregateFunctionAsync<long>(OperateType.Count, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate function query
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the value</returns>
        async Task<TValue> AggregateFunctionAsync<TValue>(OperateType funcType, IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<TEntity>(funcType);
            SetCommand(cmd, null);
            cmd.Query = query;
            TValue data = await WorkManager.AggregateValueAsync<TValue>(cmd).ConfigureAwait(false);
            return data;
        }

        #endregion

        #endregion

        #region Util

        #region Set command values

        /// <summary>
        /// Set command values
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="values">Values</param>
        void SetCommand(ICommand command, Dictionary<string, dynamic> values = null)
        {
            if (command == null)
            {
                return;
            }
            var type = typeof(TEntity);

            //set object name
            command.ObjectName = EntityManager.GetEntityObjectName(type);

            #region set primary key and values

            var primaryFields = EntityManager.GetPrimaryKeys(type);
            if (primaryFields.IsNullOrEmpty())
            {
                return;
            }
            List<string> primaryKeys = new List<string>();
            Dictionary<string, dynamic> primaryValues = new Dictionary<string, dynamic>();
            foreach (var field in primaryFields)
            {
                primaryKeys.Add(field);
                dynamic value = null;
                if (values?.TryGetValue(field, out value) ?? false)
                {
                    primaryValues.Add(field, value);
                }
            }
            command.ObjectKeys = primaryKeys;
            command.ObjectKeyValues = primaryValues;

            #endregion
        }

        #endregion

        #endregion
    }
}
