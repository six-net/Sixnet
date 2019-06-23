using EZNEW.Develop.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Framework.Extension;
using System.Linq;
using EZNEW.Framework.Fault;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Command.Modify;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// repository warehouse
    /// </summary>
    public class DataWarehouse<T> : IDataWarehouse where T : BaseEntity<T>
    {
        /// <summary>
        /// datas
        /// </summary>
        public Dictionary<string, DataPackage<T>> Datas
        {
            get; set;
        } = new Dictionary<string, DataPackage<T>>();

        /// <summary>
        /// remove querys
        /// </summary>
        List<IQuery> removeQuerys = new List<IQuery>();

        /// <summary>
        /// modify expression
        /// </summary>
        List<Tuple<IModify, IQuery>> modifyExpressions = new List<Tuple<IModify, IQuery>>();

        #region init

        /// <summary>
        /// init datas from data source
        /// </summary>
        /// <param name="datas">datas</param>
        internal void InitFromDataSource(IEnumerable<T> datas, IQuery query)
        {
            if (datas == null || !datas.Any())
            {
                return;
            }
            foreach (var data in datas)
            {
                InitFromDataSource(data, query);
            }
        }

        /// <summary>
        /// init data from data source
        /// </summary>
        /// <param name="data">data</param>
        internal DataPackage<T> InitFromDataSource(T data, IQuery query)
        {
            if (data == null)
            {
                return null;
            }
            var identityValue = data.GetIdentityValue();
            if (identityValue.IsNullOrEmpty())
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (Datas.ContainsKey(identityValue))
            {
                return null;
            }
            DataPackage<T> dataPackage = DataPackage<T>.CreatePersistentDataPackage(data, query);
            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQuerys)
            {
                var removeFunc = removeQuery?.GetQueryExpression<T>();
                isRemove = removeFunc?.Invoke(data) ?? false;
                if (isRemove)
                {
                    dataPackage.RealRemove();
                    break;
                }
            }
            if (!isRemove)
            {
                // modify values
                foreach (var modifyItem in modifyExpressions)
                {
                    var modifyFunc = modifyItem.Item2?.GetQueryExpression<T>();
                    var allowModify = modifyFunc?.Invoke(data) ?? true;
                    if (allowModify)
                    {
                        dataPackage.Modify(modifyItem.Item1);
                    }
                }
            }
            SaveDataPackage(identityValue, dataPackage);
            return dataPackage;
        }

        /// <summary>
        /// init new data
        /// </summary>
        /// <param name="datas"></param>
        internal void InitNew(IEnumerable<T> datas)
        {
            if (datas == null || !datas.Any())
            {
                return;
            }
            foreach (var data in datas)
            {
                InitNew(data);
            }
        }

        /// <summary>
        /// init new data
        /// </summary>
        /// <param name="data"></param>
        internal DataPackage<T> InitNew(T data)
        {
            if (data == null)
            {
                return null;
            }
            var identityValue = data.GetIdentityValue();
            if (identityValue.IsNullOrEmpty())
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (Datas.ContainsKey(identityValue))
            {
                return null;
            }
            DataPackage<T> dataPackage = DataPackage<T>.CreateNewDataPackage(data);

            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQuerys)
            {
                var removeFunc = removeQuery?.GetQueryExpression<T>();
                isRemove = removeFunc?.Invoke(data) ?? false;
                if (isRemove)
                {
                    dataPackage.RealRemove();
                    break;
                }
            }

            SaveDataPackage(identityValue, dataPackage);
            return dataPackage;
        }

        #endregion

        #region merge data

        /// <summary>
        /// merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="query">query </param>
        /// <param name="sort">sort </param>
        /// <returns></returns>
        public List<T> Merge(IEnumerable<T> datas, IQuery query = null, bool sort = false)
        {
            var warehouseDatas = GetWarehouseDatas(query);
            if (!datas.IsNullOrEmpty())
            {
                warehouseDatas = warehouseDatas.Except(datas).ToList();
                foreach (var data in datas)
                {
                    if (data == null)
                    {
                        continue;
                    }
                    var dataPackage = GetDataPackage(data);
                    var realData = data;
                    if (dataPackage == null)
                    {
                        InitFromDataSource(data, query);
                    }
                    else
                    {
                        realData = dataPackage.MergeFromDataSource(data, query);
                    }
                    if (realData == null)
                    {
                        continue;
                    }
                    warehouseDatas.Add(realData);
                }
            }
            if (warehouseDatas.IsNullOrEmpty())
            {
                return new List<T>(0);
            }
            if (!(query?.Orders.IsNullOrEmpty() ?? true))
            {
                warehouseDatas = query.Order(warehouseDatas).ToList();
            }
            var querySize = 0;
            if (query != null)
            {
                querySize = query.PagingInfo == null ? query.QuerySize : query.PagingInfo.PageSize;
            }
            if (querySize > 0 && warehouseDatas.Count > querySize)
            {
                warehouseDatas = warehouseDatas.Take(querySize).ToList();
            }
            return warehouseDatas;
        }

        /// <summary>
        /// merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public T Merge(T data, IQuery query = null)
        {
            var warehouseDatas = GetWarehouseDatas(query);
            //merge data to warehouse
            if (data != null)
            {
                warehouseDatas.Remove(data);
                var realData = data;
                var dataPackage = GetDataPackage(data);
                if (dataPackage == null)
                {
                    InitFromDataSource(data, query);
                }
                else
                {
                    realData = dataPackage.MergeFromDataSource(data, query);
                }
                warehouseDatas.Add(realData);
            }

            //get data
            if (warehouseDatas.IsNullOrEmpty())
            {
                return data;
            }
            else
            {
                if (!(query?.Orders.IsNullOrEmpty() ?? true))
                {
                    warehouseDatas = query.Order(warehouseDatas).ToList();
                }
                return warehouseDatas.FirstOrDefault();
            }
        }

        #endregion

        #region save

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas"></param>
        public void Save(IEnumerable<T> datas)
        {
            if (datas == null || !datas.Any())
            {
                return;
            }
            foreach (var data in datas)
            {
                Save(data);
            }
        }

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        public void Save(T data)
        {
            if (data == null)
            {
                return;
            }
            var dataPackage = GetDataPackage(data);
            if (dataPackage == null)
            {
                dataPackage = InitNew(data);
            }
            dataPackage?.Save(data);
        }

        #endregion

        #region remove

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        public void Remove(IEnumerable<T> datas)
        {
            if (datas == null || !datas.Any())
            {
                return;
            }
            foreach (var data in datas)
            {
                Remove(data);
            }
        }

        /// <summary>
        /// remote data
        /// </summary>
        /// <param name="data">data</param>
        public void Remove(T data)
        {
            var dataPackage = GetDataPackage(data);
            if (dataPackage == null)
            {
                dataPackage = InitNew(data);
            }
            dataPackage?.Remove();
        }

        /// <summary>
        /// remove query
        /// </summary>
        /// <param name="query">query</param>
        public void Remove(IQuery query)
        {
            if (query == null)
            {
                return;
            }
            var func = query.GetQueryExpression<T>();
            if (func == null)
            {
                return;
            }
            removeQuerys.Add(query);
            foreach (var dataPackage in Datas.Values)
            {
                if (func(dataPackage.WarehouseData))
                {
                    dataPackage.RealRemove();
                }
            }
        }

        #endregion

        #region modify 

        /// <summary>
        /// modify
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query</param>
        public void Modify(IModify modifyExpression, IQuery query)
        {
            if (modifyExpression == null)
            {
                return;
            }
            modifyExpressions.Add(new Tuple<IModify, IQuery>(modifyExpression, query));
            var queryFunc = query?.GetQueryExpression<T>();
            foreach (var item in Datas)
            {
                if (queryFunc?.Invoke(item.Value.WarehouseData) ?? true)
                {
                    item.Value?.Modify(modifyExpression);
                }
            }
        }

        #endregion

        #region exist

        /// <summary>
        /// determines if it contains a value
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public CheckExistResult Exist(IQuery query)
        {
            var dataPackages = GetDataPackages(query);
            CheckExistResult result = new CheckExistResult()
            {
                IsExist = dataPackages?.Any(c => c.Operate != WarehouseDataOperate.Remove) ?? false
            };
            if (!result.IsExist)
            {
                var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
                if (!sourceDatas.IsNullOrEmpty())
                {
                    QueryFactory.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
                }
                result.CheckQuery = query;
            }
            return result;
        }

        #endregion

        #region count

        /// <summary>
        /// count
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public CountResult Count(IQuery query)
        {
            var dataPackages = GetDataPackages(query);
            long newDataCount = 0;
            long removePersistentDataCount = 0;
            long allPersistentDataCount = 0;
            dataPackages.ForEach(c =>
            {
                if (c.LifeSource == DataLifeSource.New)
                {
                    newDataCount += 1;
                }
                else
                {
                    allPersistentDataCount += 1;
                    if (c.Operate == WarehouseDataOperate.Remove)
                    {
                        removePersistentDataCount += 1;
                    }
                }
            });
            return new CountResult()
            {
                NewDataCount = newDataCount,
                PersistentDataRemoveCount = removePersistentDataCount,
                PersistentDataCount = allPersistentDataCount,
                TotalDataCount = dataPackages.Count
            };
        }

        #endregion

        #region max

        /// <summary>
        /// compute max value
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public ComputeResult<VT> Max<VT>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<VT>()
                {
                    Value = default(VT),
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove);
            VT value;
            if (nowDataPackages.IsNullOrEmpty())
            {
                value = default(VT);
            }
            else
            {
                value = nowDataPackages.Max(c => c.WarehouseData.GetPropertyValue<VT>(propertyName));
            }
            var result = new ComputeResult<VT>() { Value = value };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryFactory.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region min

        /// <summary>
        /// compute min value
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public ComputeResult<VT> Min<VT>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<VT>()
                {
                    Value = default(VT),
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove);
            VT value;
            if (nowDataPackages.IsNullOrEmpty())
            {
                value = default(VT);
            }
            else
            {
                value = nowDataPackages.Min(c => c.WarehouseData.GetPropertyValue<VT>(propertyName));
            }
            var result = new ComputeResult<VT>() { Value = value };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryFactory.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region sum

        /// <summary>
        /// compute sum value
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public ComputeResult<VT> Sum<VT>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<VT>()
                {
                    Value = default(VT),
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove);
            dynamic value;
            if (nowDataPackages.IsNullOrEmpty())
            {
                value = default(VT);
            }
            else
            {
                value = nowDataPackages.Sum(c => (dynamic)c.WarehouseData.GetPropertyValue<VT>(propertyName));
            }
            var result = new ComputeResult<VT>() { Value = value };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryFactory.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region get warehouse data

        /// <summary>
        /// get warehouse data
        /// </summary>
        /// <param name="identityValue">identity value</param>
        /// <returns></returns>
        public T GetWarehouseData(string identityValue)
        {
            var dataPackage = GetDataPackage(identityValue);
            if (dataPackage == null)
            {
                return default(T);
            }
            return dataPackage.WarehouseData;
        }

        /// <summary>
        /// get data package
        /// </summary>
        /// <param name="identityValue">identity value</param>
        /// <returns></returns>
        public DataPackage<T> GetDataPackage(string identityValue)
        {
            if (identityValue.IsNullOrEmpty())
            {
                return null;
            }
            Datas.TryGetValue(identityValue, out DataPackage<T> dataPackage);
            return dataPackage;
        }

        /// <summary>
        /// get data packages
        /// </summary>
        /// <param name="identityValues">identity values</param>
        /// <returns></returns>
        public List<DataPackage<T>> GetDataPackages(IEnumerable<string> identityValues)
        {
            if (identityValues.IsNullOrEmpty())
            {
                return new List<DataPackage<T>>(0);
            }
            return Datas.Where(c => identityValues.Contains(c.Key)).Select(c => c.Value).ToList();
        }

        #endregion

        #region life source

        /// <summary>
        /// get life status
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public DataLifeSource GetLifeSource(T data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            var dataPackage = GetDataPackage(data.GetIdentityValue());
            return dataPackage?.LifeSource ?? DataLifeSource.New;
        }

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="life source">life source</param>
        public void ModifyLifeSource(T data, DataLifeSource lifeSource)
        {
            if (data == null)
            {
                return;
            }
            var dataPackage = GetDataPackage(data.GetIdentityValue());
            if (dataPackage == null)
            {
                dataPackage = InitNew(data);
            }
            dataPackage.ChangeLifeSource(lifeSource);
        }

        #endregion

        #region helper

        /// <summary>
        /// save data package
        /// </summary>
        /// <param name="identityValue">identity value</param>
        /// <param name="dataPackage">data package</param>
        void SaveDataPackage(string identityValue, DataPackage<T> dataPackage)
        {
            if (identityValue.IsNullOrEmpty() || dataPackage == null)
            {
                return;
            }
            if (Datas.ContainsKey(identityValue))
            {
                Datas[identityValue] = dataPackage;
            }
            else
            {
                Datas.Add(identityValue, dataPackage);
            }
        }

        /// <summary>
        /// get all datas
        /// </summary>
        /// <returns></returns>
        List<T> GetAllDatas()
        {
            return Datas?.Select(c => c.Value.WarehouseData).ToList() ?? new List<T>(0);
        }

        /// <summary>
        /// get stored datas
        /// </summary>
        /// <returns></returns>
        List<T> GetWarehouseDatas()
        {
            var storedDatas = Datas?.Where(c => c.Value.Operate != WarehouseDataOperate.Remove).Select(c => c.Value.WarehouseData);
            return storedDatas.ToList();
        }

        /// <summary>
        /// get stored datas
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <param name="query">query</param>
        /// <returns></returns>
        List<T> GetWarehouseDatas(IQuery query)
        {
            var storedDatas = GetWarehouseDatas();
            if (query != null)
            {
                var func = query.GetQueryExpression<T>();
                if (func != null)
                {
                    storedDatas = storedDatas.Where(func).ToList();
                }
            }
            return storedDatas;
        }

        /// <summary>
        /// get data package
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        DataPackage<T> GetDataPackage(T data)
        {
            if (data == null)
            {
                return null;
            }
            string identityValue = data.GetIdentityValue();
            return GetDataPackage(identityValue);
        }

        /// <summary>
        /// get data package
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        List<DataPackage<T>> GetDataPackages(IQuery query)
        {
            var dataPackages = Datas?.Values.ToList();
            if (dataPackages.IsNullOrEmpty())
            {
                return new List<DataPackage<T>>(0);
            }
            if (query != null)
            {
                var func = query.GetQueryExpression<T>();
                dataPackages = dataPackages.Where(c => func(c.WarehouseData)).ToList();
            }
            return dataPackages;
        }

        /// <summary>
        ///  get identity value is null or empty exception
        /// </summary>
        /// <returns></returns>
        EZNEWException IdentityValueIsNullOrEmptyException()
        {
            return new EZNEWException(string.Format("{0} identity value is null or empty", typeof(T)));
        }

        #endregion
    }
}
