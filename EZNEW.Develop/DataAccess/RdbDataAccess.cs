using EZNEW.Develop.Entity;
using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Framework.Extension;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Fault;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// imeplements data access for rdb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RdbDataAccess<T> : IDataAccess<T> where T : BaseEntity<T>, new()
    {
        static Type entityType = typeof(T);

        #region add data

        /// <summary>
        /// add data
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>icommand</returns>
        public virtual ICommand Add(T obj)
        {
            return AddAsync(obj).Result;
        }

        /// <summary>
        /// add data
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>icommand</returns>
        public virtual async Task<ICommand> AddAsync(T obj)
        {
            InitVersionFieldValue(obj);//init version field value
            InitRefreshDateFieldValue(obj);//init refresh date value
            var cmd = RdbCommand.CreateNewCommand<T>(OperateType.Insert, obj);
            SetCommand(cmd, obj.GetAllPropertyValues());
            cmd.MustReturnValueOnSuccess = true;
            return await Task.FromResult(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// add data list
        /// </summary>
        /// <param name="objList">object list</param>
        /// <returns>icommand list</returns>
        public virtual List<ICommand> Add(IEnumerable<T> objList)
        {
            return AddAsync(objList).Result;
        }

        /// <summary>
        /// add data list
        /// </summary>
        /// <param name="objList">object list</param>
        /// <returns>icommand list</returns>
        public virtual async Task<List<ICommand>> AddAsync(IEnumerable<T> objList)
        {
            if (objList == null)
            {
                return new List<ICommand>(0);
            }
            List<ICommand> cmdList = new List<ICommand>();
            foreach (var obj in objList)
            {
                cmdList.Add(await AddAsync(obj).ConfigureAwait(false));
            }
            return cmdList;
        }

        #endregion

        #region edit data

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <returns></returns>
        public virtual ICommand Modify(T newData, T oldData)
        {
            return ModifyAsync(newData, oldData).Result;
        }

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <returns></returns>
        public virtual async Task<ICommand> ModifyAsync(T newData, T oldData)
        {
            return await ModifyAsync(newData, oldData, QueryFactory.AppendEntityIdentityCondition(newData)).ConfigureAwait(false);
        }

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        public virtual ICommand Modify(T newData, T oldData, IQuery query)
        {
            return ModifyAsync(newData, oldData, query).Result;
        }

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        public virtual async Task<ICommand> ModifyAsync(T newData, T oldData, IQuery query)
        {
            Dictionary<string, dynamic> modifyValues = newData.GetModifyValues(oldData);
            if (modifyValues == null || modifyValues.Count <= 0)
            {
                return null;
            }
            if (query == null)
            {
                throw new EZNEWException("the data modification condition is null");
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(entityType);
            if (!string.IsNullOrWhiteSpace(versionFieldName))
            {
                var nowVersionValue = newData.GetPropertyValue(versionFieldName);
                if (!modifyValues.ContainsKey(versionFieldName))
                {
                    var newVersionValue = nowVersionValue + 1;
                    newData.SetPropertyValue(versionFieldName, newVersionValue);
                    modifyValues.Add(versionFieldName, newVersionValue);
                }
                query.And(versionFieldName, CriteriaOperator.Equal, nowVersionValue);
            }

            #endregion

            #region update date

            string refreshFieldName = EntityManager.GetRefreshDateField(entityType);
            if (!string.IsNullOrWhiteSpace(refreshFieldName))
            {
                if (!modifyValues.ContainsKey(refreshFieldName))
                {
                    var nowDate = DateTime.Now;
                    newData.SetPropertyValue(refreshFieldName, nowDate);
                    modifyValues.Add(refreshFieldName, nowDate);
                }
            }

            #endregion

            return await UpdateAsync(modifyValues.Keys, modifyValues, query).ConfigureAwait(false);
        }

        /// <summary>
        /// edit data with expression
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        public virtual ICommand Modify(IModify modifyExpression, IQuery query)
        {
            return ModifyAsync(modifyExpression, query).Result;
        }

        /// <summary>
        /// edit data with expression
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        public virtual async Task<ICommand> ModifyAsync(IModify modifyExpression, IQuery query)
        {
            Dictionary<string, IModifyValue> fieldAndValues = modifyExpression.GetModifyValues();
            if (fieldAndValues == null || fieldAndValues.Count <= 0)
            {
                return null;
            }

            #region control version

            string versionFieldName = EntityManager.GetVersionField(typeof(T));
            if (!string.IsNullOrWhiteSpace(versionFieldName))
            {
                if (!fieldAndValues.ContainsKey(versionFieldName))
                {
                    fieldAndValues.Add(versionFieldName, new CalculateModifyValue(CalculateOperator.Add, 1));
                }
            }

            #endregion

            #region update date

            string refreshFieldName = EntityManager.GetRefreshDateField(typeof(T));
            if (!string.IsNullOrWhiteSpace(refreshFieldName))
            {
                if (!fieldAndValues.ContainsKey(refreshFieldName))
                {
                    fieldAndValues.Add(refreshFieldName, new FixedModifyValue(DateTime.Now));
                }
            }

            #endregion

            return await UpdateAsync(fieldAndValues.Keys, fieldAndValues, query).ConfigureAwait(false);
        }

        /// <summary>
        /// edit value
        /// </summary>
        /// <param name="fields">fields</param>
        /// <param name="parameters">parameters</param>
        /// <returns></returns>
        async Task<ICommand> UpdateAsync(IEnumerable<string> fields, object parameters, IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<T>(OperateType.Update);
            SetCommand(cmd, parameters as Dictionary<string, dynamic>);
            cmd.Fields = fields.ToList();
            cmd.Parameters = parameters;
            if (query.VerifyResult != null)
            {
                cmd.MustReturnValueOnSuccess = query.VerifyResult(0);
            }
            cmd.Query = query;
            return await Task.FromResult(cmd);
        }

        #endregion

        #region delete data

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public virtual ICommand Delete(T obj)
        {
            return Delete(QueryFactory.AppendEntityIdentityCondition(obj));
        }

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public virtual async Task<ICommand> DeleteAsync(T obj)
        {
            return await DeleteAsync(QueryFactory.AppendEntityIdentityCondition(obj)).ConfigureAwait(false);
        }

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="query">delete query</param>
        /// <returns>ICommand object</returns>
        public virtual ICommand Delete(IQuery query)
        {
            return DeleteAsync(query).Result;
        }

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="query">delete query</param>
        /// <returns>ICommand object</returns>
        public virtual async Task<ICommand> DeleteAsync(IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<T>(OperateType.Delete);
            SetCommand(cmd, null);
            if (query.VerifyResult != null)
            {
                cmd.MustReturnValueOnSuccess = query.VerifyResult(0);
            }
            cmd.Query = query;
            return await Task.FromResult(cmd).ConfigureAwait(false);
        }

        #endregion

        #region query data

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        public virtual T Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        public virtual async Task<T> GetAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<T>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            T obj = await WorkFactory.QuerySingleAsync<T>(cmd).ConfigureAwait(false);
            return obj;
        }

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data list</returns>
        public List<T> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data list</returns>
        public async Task<List<T>> GetListAsync(IQuery query)
        {
            List<T> objList = await QueryListAsync(query).ConfigureAwait(false);
            return objList ?? new List<T>(0);
        }

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data list</returns>
        async Task<List<T>> QueryListAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<T>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            var objList = (await WorkFactory.QueryAsync<T>(cmd).ConfigureAwait(false)).ToList();
            return objList;
        }

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        public IPaging<T> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        public async Task<IPaging<T>> GetPagingAsync(IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<T>(OperateType.Query);
            SetCommand(cmd, null);
            cmd.Query = query;
            var objPaging = await WorkFactory.QueryPagingAsync<T>(cmd).ConfigureAwait(false);
            return objPaging;
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>whether data is exist</returns>
        public virtual bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>whether data is exist</returns>
        public virtual async Task<bool> ExistAsync(IQuery query)
        {
            var cmd = RdbCommand.CreateNewCommand<T>(OperateType.Exist);
            SetCommand(cmd, null);
            cmd.MustReturnValueOnSuccess = true;
            cmd.Query = query;
            cmd.CommandResultType = ExecuteCommandResult.ExecuteScalar;
            return await WorkFactory.QueryAsync(cmd).ConfigureAwait(false);
        }

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        public virtual DT Max<DT>(IQuery query)
        {
            return MaxAsync<DT>(query).Result;
        }

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        public virtual async Task<DT> MaxAsync<DT>(IQuery query)
        {
            return await AggregateFunctionAsync<DT>(OperateType.Max, query).ConfigureAwait(false);
        }

        /// <summary>
        /// get minvalue
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>minvalue</returns>
        public DT Min<DT>(IQuery query)
        {
            return MinAsync<DT>(query).Result;
        }

        /// <summary>
        /// get minvalue
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>minvalue</returns>
        public async Task<DT> MinAsync<DT>(IQuery query)
        {
            return await AggregateFunctionAsync<DT>(OperateType.Min, query).ConfigureAwait(false);
        }

        /// <summary>
        /// caculate sum
        /// </summary>
        /// <typeparam name="DT">data value</typeparam>
        /// <param name="query">query value</param>
        /// <returns>caculated value</returns>
        public DT Sum<DT>(IQuery query)
        {
            return SumAsync<DT>(query).Result;
        }

        /// <summary>
        /// caculate sum
        /// </summary>
        /// <typeparam name="DT">data value</typeparam>
        /// <param name="query">query value</param>
        /// <returns>caculated value</returns>
        public async Task<DT> SumAsync<DT>(IQuery query)
        {
            return await AggregateFunctionAsync<DT>(OperateType.Sum, query).ConfigureAwait(false);
        }

        /// <summary>
        /// caculate average
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        public DT Avg<DT>(IQuery query)
        {
            return AvgAsync<DT>(query).Result;
        }

        /// <summary>
        /// caculate average
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        public async Task<DT> AvgAsync<DT>(IQuery query)
        {
            return await AggregateFunctionAsync<DT>(OperateType.Avg, query).ConfigureAwait(false);
        }

        /// <summary>
        /// caculate count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data count</returns>
        public long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// caculate count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data count</returns>
        public async Task<long> CountAsync(IQuery query)
        {
            return await AggregateFunctionAsync<long>(OperateType.Count, query).ConfigureAwait(false);
        }

        /// <summary>
        /// aggregate function query
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        async Task<DT> AggregateFunctionAsync<DT>(OperateType funcType, IQuery query)
        {
            ICommand cmd = RdbCommand.CreateNewCommand<T>(funcType);
            SetCommand(cmd, null);
            cmd.Query = query;
            DT obj = await WorkFactory.AggregateValueAsync<DT>(cmd).ConfigureAwait(false);
            return obj;
        }

        #endregion

        #region set command values

        /// <summary>
        /// set command values
        /// </summary>
        /// <param name="cmd">command</param>
        /// <param name="values">values</param>
        void SetCommand(ICommand cmd, Dictionary<string, dynamic> values = null)
        {
            if (cmd == null)
            {
                return;
            }
            var type = typeof(T);

            //set object name
            cmd.ObjectName = EntityManager.GetEntityObjectName(type);

            #region set primary key and values

            var primaryFields = EntityManager.GetPrimaryKeys(type);
            if (primaryFields.IsNullOrEmpty())
            {
                return;
            }
            List<string> primaryKeys = new List<string>(primaryFields.Count);
            Dictionary<string, dynamic> primaryValues = new Dictionary<string, dynamic>(primaryFields.Count);
            foreach (var field in primaryFields)
            {
                string primaryKey = field.PropertyName;
                primaryKeys.Add(primaryKey);
                dynamic value = null;
                if (values?.TryGetValue(primaryKey, out value) ?? false)
                {
                    primaryValues.Add(primaryKey, value);
                }
            }
            cmd.ObjectKeys = primaryKeys;
            cmd.ObjectKeyValues = primaryValues;

            #endregion
        }

        #endregion

        #region init datavalue

        /// <summary>
        /// init version value
        /// </summary>
        internal void InitVersionFieldValue(T obj)
        {
            if (obj == null)
            {
                return;
            }
            string versionField = EntityManager.GetVersionField(typeof(T));
            if (string.IsNullOrWhiteSpace(versionField))
            {
                return;
            }
            long initValue = 1;
            obj.SetPropertyValue(versionField, initValue);
        }

        /// <summary>
        /// init refresh date
        /// </summary>
        /// <param name="obj"></param>
        internal void InitRefreshDateFieldValue(T obj)
        {
            if (obj == null)
            {
                return;
            }
            string refreshDateField = EntityManager.GetRefreshDateField(typeof(T));
            if (string.IsNullOrWhiteSpace(refreshDateField))
            {
                return;
            }
            obj.SetPropertyValue(refreshDateField, DateTime.Now);
        }

        #endregion
    }
}
