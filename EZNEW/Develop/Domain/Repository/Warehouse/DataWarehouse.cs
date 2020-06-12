using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Fault;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Command.Modify;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Repository data warehouse
    /// </summary>
    public class DataWarehouse<TEntity> : IDataWarehouse where TEntity : BaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Gets the datas
        /// </summary>
        public Dictionary<string, DataPackage<TEntity>> DataDictionary
        {
            get; private set;
        } = new Dictionary<string, DataPackage<TEntity>>();

        /// <summary>
        /// Remove querys
        /// </summary>
        readonly List<IQuery> removeQueryCollection = new List<IQuery>();

        /// <summary>
        /// Modify expression
        /// </summary>
        readonly List<Tuple<IModify, IQuery>> modifyExpressionCollection = new List<Tuple<IModify, IQuery>>();

        #region Init

        /// <summary>
        /// Init datas from data source
        /// </summary>
        /// <param name="datas">Entity datas</param>
        /// <param name="query">Query object</param>
        internal void InitFromDataSource(IEnumerable<TEntity> datas, IQuery query)
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
        /// Init data from data source
        /// </summary>
        /// <param name="data">Data</param>
        internal DataPackage<TEntity> InitFromDataSource(TEntity data, IQuery query)
        {
            if (data == null)
            {
                return null;
            }
            var identityValue = data.GetIdentityValue();
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (DataDictionary.ContainsKey(identityValue))
            {
                return null;
            }
            DataPackage<TEntity> dataPackage = DataPackage<TEntity>.CreatePersistentDataPackage(data, query);
            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQueryCollection)
            {
                var removeFunc = removeQuery?.GetQueryExpression<TEntity>();
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
                foreach (var modifyItem in modifyExpressionCollection)
                {
                    var modifyFunc = modifyItem.Item2?.GetQueryExpression<TEntity>();
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
        /// Init new data
        /// </summary>
        /// <param name="datas">Datas</param>
        internal void InitNew(IEnumerable<TEntity> datas)
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
        /// Init new data
        /// </summary>
        /// <param name="data">Data</param>
        internal DataPackage<TEntity> InitNew(TEntity data)
        {
            if (data == null)
            {
                return null;
            }
            var identityValue = data.GetIdentityValue();
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (DataDictionary.ContainsKey(identityValue))
            {
                return null;
            }
            DataPackage<TEntity> dataPackage = DataPackage<TEntity>.CreateNewDataPackage(data);

            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQueryCollection)
            {
                var removeFunc = removeQuery?.GetQueryExpression<TEntity>();
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

        #region Merge data

        /// <summary>
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="query">Query </param>
        /// <param name="sort">Sort </param>
        /// <returns>Return entity data list</returns>
        public List<TEntity> Merge(IEnumerable<TEntity> datas, IQuery query = null, bool sort = false)
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
                        dataPackage = InitFromDataSource(data, query);
                    }
                    else
                    {
                        realData = dataPackage.MergeFromDataSource(data, query);
                    }
                    if (dataPackage.Operate == WarehouseDataOperate.Remove || realData == null)
                    {
                        continue;
                    }
                    warehouseDatas.Add(realData);
                }
            }
            if (warehouseDatas.IsNullOrEmpty())
            {
                return new List<TEntity>(0);
            }
            if (!(query?.Orders.IsNullOrEmpty() ?? true))
            {
                warehouseDatas = query.Sort(warehouseDatas).ToList();
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
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return warehouse entity data</returns>
        public TEntity Merge(TEntity data, IQuery query = null)
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
                    dataPackage = InitFromDataSource(data, query);
                }
                else
                {
                    realData = dataPackage.MergeFromDataSource(data, query);
                }
                if (dataPackage.Operate != WarehouseDataOperate.Remove && realData != null)
                {
                    warehouseDatas.Add(realData);
                }
            }

            //get data
            if (warehouseDatas.IsNullOrEmpty())
            {
                return default;
            }
            else
            {
                if (!(query?.Orders.IsNullOrEmpty() ?? true))
                {
                    warehouseDatas = query.Sort(warehouseDatas).ToList();
                }
                return warehouseDatas.FirstOrDefault();
            }
        }

        #endregion

        #region Save

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="datas">Datas</param>
        public void Save(IEnumerable<TEntity> datas)
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
        /// Save data
        /// </summary>
        /// <param name="data">Data</param>
        public void Save(TEntity data)
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

        #region Remove

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        public void Remove(IEnumerable<TEntity> datas)
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
        /// Remove data
        /// </summary>
        /// <param name="data">Data</param>
        public void Remove(TEntity data)
        {
            var dataPackage = GetDataPackage(data);
            if (dataPackage == null)
            {
                dataPackage = InitNew(data);
            }
            dataPackage?.Remove();
        }

        /// <summary>
        /// Remove by query
        /// </summary>
        /// <param name="query">Query</param>
        public void Remove(IQuery query)
        {
            if (query == null)
            {
                return;
            }
            var func = query.GetQueryExpression<TEntity>();
            if (func == null)
            {
                return;
            }
            removeQueryCollection.Add(query);
            foreach (var dataPackage in DataDictionary.Values)
            {
                if (func(dataPackage.WarehouseData))
                {
                    dataPackage.RealRemove();
                }
            }
        }

        #endregion

        #region Modify 

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        public void Modify(IModify modifyExpression, IQuery query)
        {
            if (modifyExpression == null)
            {
                return;
            }
            modifyExpressionCollection.Add(new Tuple<IModify, IQuery>(modifyExpression, query));
            var queryFunc = query?.GetQueryExpression<TEntity>();
            foreach (var item in DataDictionary)
            {
                if (queryFunc?.Invoke(item.Value.WarehouseData) ?? true)
                {
                    item.Value?.Modify(modifyExpression);
                }
            }
        }

        #endregion

        #region Exist

        /// <summary>
        /// Determines if it contains a value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the check result</returns>
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
                    query = QueryManager.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
                }
                result.CheckQuery = query;
            }
            return result;
        }

        #endregion

        #region Count

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the count result</returns>
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

        #region Max

        /// <summary>
        /// Compute max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public ComputeResult<TValue> Max<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<TValue>()
                {
                    Value = default,
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove);
            TValue value = default;
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                value = nowDataPackages.Max(c => c.WarehouseData.GetPropertyValue<TValue>(propertyName));
                validValue = true;
            }
            var result = new ComputeResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryManager.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region Min

        /// <summary>
        /// Compute min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public ComputeResult<TValue> Min<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<TValue>()
                {
                    Value = default(TValue),
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove);
            TValue value = default;
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                value = nowDataPackages.Min(c => c.WarehouseData.GetPropertyValue<TValue>(propertyName));
                validValue = true;
            }
            var result = new ComputeResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryManager.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region Sum

        /// <summary>
        /// Compute sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return compute result</returns>
        public ComputeResult<TValue> Sum<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new ComputeResult<TValue>()
                {
                    Value = default,
                    ComputeQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operate != WarehouseDataOperate.Remove).ToList();
            dynamic value = default(TValue);
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                nowDataPackages.ForEach(c =>
                {
                    value += c.WarehouseData.GetPropertyValue<TValue>(propertyName);
                });
                validValue = true;
            }
            var result = new ComputeResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            var sourceDatas = dataPackages.Where(c => c.LifeSource == DataLifeSource.DataSource);
            if (!sourceDatas.IsNullOrEmpty())
            {
                QueryManager.AppendEntityIdentityCondition(sourceDatas.Select(c => c.PersistentData), query, true);
            }
            result.ComputeQuery = query;
            return result;
        }

        #endregion

        #region Get warehouse data

        /// <summary>
        /// Get warehouse data
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <returns>Return the entity warehouse data</returns>
        public TEntity GetWarehouseData(string identityValue)
        {
            var dataPackage = GetDataPackage(identityValue);
            if (dataPackage == null)
            {
                return default(TEntity);
            }
            return dataPackage.WarehouseData;
        }

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <returns>Return the entity package</returns>
        public DataPackage<TEntity> GetDataPackage(string identityValue)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                return null;
            }
            DataDictionary.TryGetValue(identityValue, out DataPackage<TEntity> dataPackage);
            return dataPackage;
        }

        /// <summary>
        /// Get data packages
        /// </summary>
        /// <param name="identityValues">identity values</param>
        /// <returns>Return the entity packages</returns>
        public List<DataPackage<TEntity>> GetDataPackages(IEnumerable<string> identityValues)
        {
            if (identityValues.IsNullOrEmpty())
            {
                return new List<DataPackage<TEntity>>(0);
            }
            return DataDictionary.Where(c => identityValues.Contains(c.Key)).Select(c => c.Value).ToList();
        }

        #endregion

        #region Life source

        /// <summary>
        /// Get life status
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return entity life source</returns>
        public DataLifeSource GetLifeSource(TEntity data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            var dataPackage = GetDataPackage(data.GetIdentityValue());
            return dataPackage?.LifeSource ?? DataLifeSource.New;
        }

        /// <summary>
        /// Modify life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        public void ModifyLifeSource(TEntity data, DataLifeSource lifeSource)
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

        #region Util

        /// <summary>
        /// Save data package
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <param name="dataPackage">Data package</param>
        void SaveDataPackage(string identityValue, DataPackage<TEntity> dataPackage)
        {
            if (string.IsNullOrWhiteSpace(identityValue) || dataPackage == null)
            {
                return;
            }
            if (DataDictionary.ContainsKey(identityValue))
            {
                DataDictionary[identityValue] = dataPackage;
            }
            else
            {
                DataDictionary.Add(identityValue, dataPackage);
            }
        }

        /// <summary>
        /// Get stored datas
        /// </summary>
        /// <returns></returns>
        List<TEntity> GetWarehouseDatas()
        {
            var storedDatas = DataDictionary?.Where(c => c.Value.Operate != WarehouseDataOperate.Remove).Select(c => c.Value.WarehouseData);
            return storedDatas.ToList();
        }

        /// <summary>
        /// Get stored datas
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data list</returns>
        List<TEntity> GetWarehouseDatas(IQuery query)
        {
            var storedDatas = GetWarehouseDatas();
            if (query != null)
            {
                var func = query.GetQueryExpression<TEntity>();
                if (func != null)
                {
                    storedDatas = storedDatas.Where(func).ToList();
                }
            }
            return storedDatas;
        }

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data package</returns>
        DataPackage<TEntity> GetDataPackage(TEntity data)
        {
            if (data == null)
            {
                return null;
            }
            string identityValue = data.GetIdentityValue();
            return GetDataPackage(identityValue);
        }

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Return the entity data packages</returns>
        List<DataPackage<TEntity>> GetDataPackages(IQuery query)
        {
            var dataPackages = DataDictionary?.Values.ToList();
            if (dataPackages.IsNullOrEmpty())
            {
                return new List<DataPackage<TEntity>>(0);
            }
            if (query != null)
            {
                var func = query.GetQueryExpression<TEntity>();
                dataPackages = dataPackages.Where(c => func(c.WarehouseData)).ToList();
            }
            return dataPackages;
        }

        /// <summary>
        ///  Get identity value is null or empty exception
        /// </summary>
        /// <returns></returns>
        EZNEWException IdentityValueIsNullOrEmptyException()
        {
            return new EZNEWException(string.Format("{0} identity value is null or empty", typeof(TEntity)));
        }

        #endregion
    }
}
