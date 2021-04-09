using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Fault;
using EZNEW.DependencyInjection;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Repository datawarehouse manager
    /// </summary>
    public static class WarehouseManager
    {
        #region Properties

        /// <summary>
        /// Whether enable dev debug
        /// </summary>
        static bool devDebug = false;

        #endregion

        #region Enable warehouse

        /// <summary>
        /// Enable debug warehouse
        /// </summary>
        public static void EnableDebugWarehouse()
        {
            devDebug = true;
        }

        #endregion

        #region Register default warehouse

        /// <summary>
        /// Register default warehouse
        /// </summary>
        internal static void RegisterDefaultWarehouse<TEntity, TDataAccess>() where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
        {
            if (ContainerManager.IsRegister<IRepositoryWarehouse<TEntity, TDataAccess>>())
            {
                return;
            }
            if (devDebug)
            {
                ContainerManager.Register<IRepositoryWarehouse<TEntity, TDataAccess>, DebugRepositoryWarehouse<TEntity, TDataAccess>>();
            }
            else
            {
                ContainerManager.Register<IRepositoryWarehouse<TEntity, TDataAccess>, DefaultRepositoryWarehouse<TEntity, TDataAccess>>();
            }
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="datas">New datas</param>
        /// <param name="query">Query object</param>
        /// <returns>Return the newest data</returns>
        public static List<TEntity> Merge<TEntity>(IEnumerable<TEntity> datas, IQuery query = null, bool sort = false) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Merge(datas, query, sort) ?? datas?.ToList() ?? new List<TEntity>(0);
        }

        /// <summary>
        /// Merge paging
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="originalPaging">Original paging</param>
        /// <param name="query">Query object</param>
        /// <returns>Return the newest data paging</returns>
        public static PagingInfo<TEntity> MergePaging<TEntity>(PagingInfo<TEntity> originalPaging, IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            if (originalPaging == null)
            {
                originalPaging = Pager.Empty<TEntity>();
            }
            var totalCount = originalPaging.TotalCount;
            var dataCount = originalPaging.Items.GetCount();
            var datas = Merge(originalPaging.Items, query, true);
            var newDataCount = datas.Count;
            var diffCount = newDataCount - dataCount;
            totalCount += diffCount;
            if (newDataCount > originalPaging.PageSize)
            {
                datas = datas.Take(originalPaging.PageSize).ToList();
            }
            return Pager.Create(originalPaging.Page, originalPaging.PageSize, totalCount, datas);
        }

        /// <summary>
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="query">Query object</param>
        /// <returns>Return the newest data</returns>
        public static TEntity Merge<TEntity>(TEntity data, IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            if (warehouse == null)
            {
                return data;
            }
            return warehouse.Merge(data, query);
        }

        #endregion

        #region Exist

        /// <summary>
        /// Determines if it contains a value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return check exist result</returns>
        public static CheckExistResult Exist<TEntity>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Exist(query) ?? new CheckExistResult() { IsExist = false, CheckQuery = query };
        }

        #endregion

        #region Save

        /// <summary>
        /// Save data
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns></returns>
        public static void Save<TEntity>(params TEntity[] datas) where TEntity : BaseEntity<TEntity>, new()
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var warehouse = GetWarehouse<TEntity>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            foreach (var data in datas)
            {
                warehouse.Save(data);
            }
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns></returns>
        public static void Remove<TEntity>(params TEntity[] datas) where TEntity : BaseEntity<TEntity>, new()
        {
            if (datas == null)
            {
                return;
            }
            var warehouse = GetWarehouse<TEntity>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            foreach (var data in datas)
            {
                warehouse.Remove(data);
            }
        }

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns></returns>
        public static void Remove<TEntity>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            warehouse.Remove(query);
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify 
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <returns></returns>
        public static void Modify<TEntity>(IModify modifyExpression, IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            if (warehouse == null)
            {
                throw NotInitUnitWork();
            }
            warehouse.Modify(modifyExpression, query);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns></returns>
        public static CountResult Count<TEntity>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Count(query) ?? new CountResult();
        }

        #endregion

        #region Max

        /// <summary>
        /// Max value
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public static ComputeResult<TValue> Max<TEntity, TValue>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Max<TValue>(query) ?? new ComputeResult<TValue>() { Value = default, ComputeQuery = query };
        }

        #endregion

        #region Min

        /// <summary>
        /// Min value
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return compute result</returns>
        public static ComputeResult<TValue> Min<TEntity, TValue>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Min<TValue>(query) ?? new ComputeResult<TValue>() { Value = default(TValue), ComputeQuery = query };
        }

        #endregion

        #region Sum

        /// <summary>
        /// Sum value
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public static ComputeResult<TValue> Sum<TEntity, TValue>(IQuery query) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.Sum<TValue>(query) ?? new ComputeResult<TValue>() { Value = default(TValue), ComputeQuery = query };
        }

        #endregion

        #region Life source

        /// <summary>
        /// Get life status
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return the data life source</returns>
        public static DataLifeSource GetLifeSource<TEntity>(TEntity data) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.GetLifeSource(data) ?? DataLifeSource.New;
        }

        /// <summary>
        /// Modify life source
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        public static void ModifyLifeSource<TEntity>(TEntity data, DataLifeSource lifeSource) where TEntity : BaseEntity<TEntity>, new()
        {
            var warehouse = GetWarehouse<TEntity>();
            warehouse?.ModifyLifeSource(data, lifeSource);
        }

        #endregion

        #region Get warehoue

        /// <summary>
        /// Get warehouse
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return the data warehouse</returns>
        public static DataWarehouse<TEntity> GetWarehouse<TEntity>() where TEntity : BaseEntity<TEntity>, new()
        {
            return WorkManager.Current?.GetWarehouse<TEntity>();
        }

        #endregion

        #region Get datapackage

        /// <summary>
        /// Get data package
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="identityValue">Identity value</param>
        /// <returns>Return the data package</returns>
        public static DataPackage<TEntity> GetDataPackage<TEntity>(string identityValue) where TEntity : BaseEntity<TEntity>, new()
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                return null;
            }
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.GetDataPackage(identityValue);
        }

        /// <summary>
        /// Get data packages
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="identityValues">Identity values</param>
        /// <returns>Return data packages</returns>
        public static List<DataPackage<TEntity>> GetDataPackages<TEntity>(IEnumerable<string> identityValues) where TEntity : BaseEntity<TEntity>, new()
        {
            if (identityValues.IsNullOrEmpty())
            {
                return new List<DataPackage<TEntity>>(0);
            }
            var warehouse = GetWarehouse<TEntity>();
            return warehouse?.GetDataPackages(identityValues);
        }

        #endregion

        #region Get not init unit work exception

        static EZNEWException NotInitUnitWork()
        {
            return new EZNEWException("didn't init unit work");
        }

        #endregion
    }
}
