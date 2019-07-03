using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using EZNEW.Framework.IoC;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// repository warehouse
    /// </summary>
    public static class WarehouseManager
    {
        #region merge

        /// <summary>
        /// merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <param name="datas">now datas</param>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public static List<ET> Merge<ET>(IEnumerable<ET> datas, IQuery query = null, bool sort = false) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            return warehouse?.Merge(datas, query, sort) ?? datas?.ToList() ?? new List<ET>(0);
        }

        /// <summary>
        /// merge paging
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="originPaging">origin paging</param>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static IPaging<ET> MergePaging<ET>(IPaging<ET> originPaging, IQuery query) where ET : BaseEntity<ET>
        {
            if (originPaging == null)
            {
                originPaging = Paging<ET>.EmptyPaging();
            }
            var totalCount = originPaging.TotalCount;
            var dataCount = originPaging.Count();
            var datas = Merge(originPaging, query, true);
            var newDataCount = datas.Count;
            var diffCount = newDataCount - dataCount;
            totalCount += diffCount;
            if (newDataCount > originPaging.PageSize)
            {
                datas = datas.Take(originPaging.PageSize).ToList();
            }
            return new Paging<ET>(originPaging.Page, originPaging.PageSize, totalCount, datas);
        }

        /// <summary>
        /// merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <typeparam name="MT"></typeparam>
        /// <param name="repositoryType">repository type</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ET Merge<ET>(ET data, IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            return warehouse?.Merge(data, query);
        }

        #endregion

        #region exist

        /// <summary>
        /// determines if it contains a value
        /// </summary>
        /// <param name="repositoryType">repository type</param>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public static async Task<CheckExistResult> ExistAsync<ET>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            var result = warehouse?.Exist(query) ?? new CheckExistResult() { IsExist = false, CheckQuery = query };
            return await Task.FromResult(result);
        }

        #endregion

        #region save

        /// <summary>
        /// save data
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static async Task SaveAsync<ET>(params ET[] datas) where ET : BaseEntity<ET>
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var warehouse = GetWarehouse<ET>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            foreach (var data in datas)
            {
                warehouse.Save(data);
            }
            await Task.CompletedTask;
        }

        #endregion

        #region remove

        /// <summary>
        /// remove data
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static async Task RemoveAsync<ET>(params ET[] datas) where ET : BaseEntity<ET>
        {
            if (datas == null)
            {
                return;
            }
            var warehouse = GetWarehouse<ET>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            foreach (var data in datas)
            {
                warehouse.Remove(data);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task RemoveAsync<ET>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            warehouse.Remove(query);
            await Task.CompletedTask;
        }

        #endregion

        #region modify

        /// <summary>
        /// modify 
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task ModifyAsync<ET>(IModify modifyExpression, IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            warehouse.Modify(modifyExpression, query);
            await Task.CompletedTask;
        }

        #endregion

        #region count

        /// <summary>
        /// count
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task<CountResult> CountAsync<ET>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            var result = warehouse?.Count(query) ?? new CountResult();
            return await Task.FromResult(result);
        }

        #endregion

        #region max

        /// <summary>
        /// max value
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <typeparam name="VT">value type</typeparam>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task<ComputeResult<VT>> MaxAsync<ET, VT>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            var result = warehouse?.Max<VT>(query) ?? new ComputeResult<VT>() { Value = default(VT), ComputeQuery = query };
            return await Task.FromResult(result);
        }

        #endregion

        #region min

        /// <summary>
        /// min value
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <typeparam name="VT">value type</typeparam>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task<ComputeResult<VT>> MinAsync<ET, VT>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            var result = warehouse?.Min<VT>(query) ?? new ComputeResult<VT>() { Value = default(VT), ComputeQuery = query };
            return await Task.FromResult(result);
        }

        #endregion

        #region sum

        /// <summary>
        /// min value
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <typeparam name="VT">value type</typeparam>
        /// <param name="query">query</param>
        /// <returns></returns>
        public static async Task<ComputeResult<VT>> SumAsync<ET, VT>(IQuery query) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            var result = warehouse?.Sum<VT>(query) ?? new ComputeResult<VT>() { Value = default(VT), ComputeQuery = query };
            return await Task.FromResult(result);
        }

        #endregion

        #region life source

        /// <summary>
        /// get life status
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public static DataLifeSource GetLifeSource<ET>(ET data) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            return warehouse?.GetLifeSource(data) ?? DataLifeSource.New;
        }

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="life source">life source</param>
        public static void ModifyLifeSource<ET>(ET data, DataLifeSource lifeSource) where ET : BaseEntity<ET>
        {
            var warehouse = GetWarehouse<ET>();
            warehouse?.ModifyLifeSource(data, lifeSource);
        }

        #endregion

        #region get warehoue

        /// <summary>
        /// get warehouse
        /// </summary>
        /// <typeparam name="RT"></typeparam>
        /// <returns></returns>
        public static DataWarehouse<ET> GetWarehouse<ET>() where ET : BaseEntity<ET>
        {
            return WorkFactory.Current?.GetWarehouse<ET>();
        }

        #endregion

        #region get datapackage

        /// <summary>
        /// get data package
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="identityValue"></param>
        /// <returns></returns>
        public static DataPackage<ET> GetDataPackage<ET>(string identityValue) where ET : BaseEntity<ET>
        {
            if (identityValue.IsNullOrEmpty())
            {
                return null;
            }
            var warehouse = GetWarehouse<ET>();
            return warehouse?.GetDataPackage(identityValue);
        }

        /// <summary>
        /// get data packages
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <param name="identityValues"></param>
        /// <returns></returns>
        public static List<DataPackage<ET>> GetDataPackages<ET>(IEnumerable<string> identityValues) where ET : BaseEntity<ET>
        {
            if (identityValues.IsNullOrEmpty())
            {
                return new List<DataPackage<ET>>(0);
            }
            var warehouse = GetWarehouse<ET>();
            return warehouse?.GetDataPackages(identityValues);
        }

        #endregion

        #region get not init unit work

        static EZNEWException NotInitUnitWork()
        {
            return new EZNEWException("didn't init unit work");
        }

        #endregion
    }
}
