using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Repository Base
    /// </summary>
    public abstract class BaseAggregationRepository<DT>
    {
        /// <summary>
        /// Save Objects
        /// </summary>
        /// <param name="objects">objects</param>
        public abstract void Save(params DT[] objects);

        /// <summary>
        /// Save Objects
        /// </summary>
        /// <param name="objects">objects</param>
        public abstract Task SaveAsync(params DT[] objects);

        /// <summary>
        /// Remove Objects
        /// </summary>
        /// <param name="objects">objects</param>
        public abstract void Remove(params DT[] objects);

        /// <summary>
        /// Remove Objects
        /// </summary>
        /// <param name="objects">objects</param>
        public abstract Task RemoveAsync(params DT[] objects);

        /// <summary>
        /// Remove Object
        /// </summary>
        /// <param name="query">query model</param>
        public abstract void Remove(IQuery query);

        /// <summary>
        /// Remove Object
        /// </summary>
        /// <param name="query">query model</param>
        public abstract Task RemoveAsync(IQuery query);

        /// <summary>
        /// Modify Object
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        public abstract void Modify(IModify expression, IQuery query);

        /// <summary>
        /// Modify Object
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        public abstract Task ModifyAsync(IModify expression, IQuery query);

        /// <summary>
        /// Get Object
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object</returns>
        public abstract DT Get(IQuery query);

        /// <summary>
        /// Get Object
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object</returns>
        public abstract Task<DT> GetAsync(IQuery query);

        /// <summary>
        /// Get Object List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public abstract List<DT> GetList(IQuery query);

        /// <summary>
        /// Get Object List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public abstract Task<List<DT>> GetListAsync(IQuery query);

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public abstract IPaging<DT> GetPaging(IQuery query);

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public abstract Task<IPaging<DT>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Wheather Have Any Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract bool Exist(IQuery query);

        /// <summary>
        /// Wheather Have Any Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract Task<bool> ExistAsync(IQuery query);

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract long Count(IQuery query);

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>Max Value</returns>
        public abstract VT Max<VT>(IQuery query);

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>Max Value</returns>
        public abstract Task<VT> MaxAsync<VT>(IQuery query);

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>Min Value</returns>
        public abstract VT Min<VT>(IQuery query);

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>Min Value</returns>
        public abstract Task<VT> MinAsync<VT>(IQuery query);

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum</returns>
        public abstract VT Sum<VT>(IQuery query);

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum</returns>
        public abstract Task<VT> SumAsync<VT>(IQuery query);

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average model</returns>
        public abstract VT Avg<VT>(IQuery query);

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average model</returns>
        public abstract Task<VT> AvgAsync<VT>(IQuery query);

        /// <summary>
        /// get life source
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public abstract DataLifeSource GetLifeSource(IAggregationRoot data);

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        public abstract void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource);
    }
}
