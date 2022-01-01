using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Development.Command;
using EZNEW.Data.Modification;
using EZNEW.Development.UnitOfWork;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
using EZNEW.Paging;

namespace EZNEW.Development.DataAccess
{
    /// <summary>
    /// Defines default data access
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class DefaultDataAccess<TEntity> : BaseDataAccess<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        #region Methods

        #region  Execute adding

        /// <summary>
        /// Execute adding data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return add command</returns>
        protected override ICommand ExecuteAdding(TEntity data)
        {
            var commandParameters = data.GetCommandParameters();
            var cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Insert, commandParameters);
            SetCommand(cmd, commandParameters);
            cmd.MustAffectedData = true;
            return cmd;
        }

        #endregion

        #region Execute modification

        /// <summary>
        /// Execute data modification
        /// </summary>
        /// <param name="originalValues">Original values</param>
        /// <param name="newValues">New values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modification command</returns>
        protected override ICommand ExecuteDataModification(Dictionary<string, dynamic> originalValues, Dictionary<string, dynamic> newValues, IQuery query)
        {
            return Update(newValues.Keys, CommandParameters.Parse(newValues), query);
        }

        /// <summary>
        /// Execute modification
        /// </summary>
        /// <param name="modificationValues">Modification values</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modification command</returns>
        protected override ICommand ExecuteModification(Dictionary<string, IModificationValue> modificationValues, IQuery query)
        {
            return Update(modificationValues.Keys, CommandParameters.Parse(modificationValues), query);
        }

        /// <summary>
        /// Update value
        /// </summary>
        /// <param name="fields">Modification fields</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modification command</returns>
        ICommand Update(IEnumerable<string> fields, CommandParameters parameters, IQuery query)
        {
            var cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Update);
            cmd.Parameters = parameters;
            cmd.Fields = fields;
            SetCommand(cmd, parameters);
            cmd.MustAffectedData = query?.MustAffectData ?? false;
            cmd.Query = query;
            return cmd;
        }

        #endregion

        #region Execute deletion

        /// <summary>
        /// Execute delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return deletion command</returns>
        protected override ICommand ExecuteDataDeletion(TEntity data)
        {
            return ExecuteDeletion(QueryManager.AppendEntityIdentityCondition(data));
        }

        /// <summary>
        /// Execute deletion
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return deletion command</returns>
        protected override ICommand ExecuteDeletion(IQuery query)
        {
            //Whether is logic delete
            var obsoleteField = EntityManager.GetObsoleteField(entityType);
            if (!string.IsNullOrWhiteSpace(obsoleteField))
            {
                var modifyExp = ModificationFactory.Create();
                modifyExp.Set(obsoleteField, true);
                return Modify(modifyExp, query);
            }

            var cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Delete);
            SetCommand(cmd, null);
            cmd.MustAffectedData = query?.MustAffectData ?? false;
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
            ICommand cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Query);
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
            ICommand cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Query);
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
        protected override async Task<PagingInfo<TEntity>> ExecuteGetPagingAsync(IQuery query)
        {
            ICommand cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Query);
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
        protected override async Task<bool> ExecuteExistsAsync(IQuery query)
        {
            var cmd = DefaultCommand.Create<TEntity>(CommandOperationType.Exist);
            SetCommand(cmd, null);
            cmd.MustAffectedData = true;
            cmd.Query = query;
            cmd.CommandResultType = CommandResultType.ScalarValue;
            return await WorkManager.ExistsAsync(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected override async Task<TValue> ExecuteMaxAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(CommandOperationType.Max, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected override async Task<TValue> ExecuteMinAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(CommandOperationType.Min, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected override async Task<TValue> ExecuteSumAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(CommandOperationType.Sum, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get avg value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the avg value</returns>
        protected override async Task<TValue> ExecuteAvgAsync<TValue>(IQuery query)
        {
            return await AggregateFunctionAsync<TValue>(CommandOperationType.Avg, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute get count value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        protected override async Task<long> ExecuteCountAsync(IQuery query)
        {
            return await AggregateFunctionAsync<long>(CommandOperationType.Count, query).ConfigureAwait(false);
        }

        /// <summary>
        /// Aggregate function query
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the value</returns>
        async Task<TValue> AggregateFunctionAsync<TValue>(CommandOperationType funcType, IQuery query)
        {
            ICommand cmd = DefaultCommand.Create<TEntity>(funcType);
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
        /// <param name="commandParameters">Parameters</param>
        void SetCommand(ICommand command, CommandParameters commandParameters)
        {
            if (command == null)
            {
                return;
            }
            var type = typeof(TEntity);

            //set object name
            command.EntityObjectName = EntityManager.GetEntityObjectName(type);

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
                ParameterItem parameterItem = null;
                if (commandParameters?.Items.TryGetValue(field, out parameterItem) ?? false)
                {
                    primaryValues.Add(field, parameterItem?.Value);
                }
            }
            command.EntityIdentityValues = primaryValues;

            #endregion
        }

        #endregion

        #endregion
    }
}
