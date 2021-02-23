using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Fault;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Data package
    /// </summary>
    public class DataPackage<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        internal DataPackage() { }

        /// <summary>
        /// Gets or sets the warehouse data
        /// </summary>
        public TEntity WarehouseData { get; set; }

        /// <summary>
        /// Gets or sets the stored data
        /// </summary>
        public TEntity PersistentData { get; set; }

        /// <summary>
        /// Gets or sets the operate
        /// </summary>
        public WarehouseDataOperate Operate { get; set; } = WarehouseDataOperate.None;

        /// <summary>
        /// Gets or sets whether is remove by condition
        /// </summary>
        public bool IsRealRemove { get; private set; } = false;

        /// <summary>
        /// Gets or sets the stored from
        /// </summary>
        public DataLifeSource LifeSource { get; private set; } = DataLifeSource.DataSource;

        /// <summary>
        /// Gets or sets the query fields
        /// </summary>
        public List<string> QueryFields { get; set; } = new List<string>();

        /// <summary>
        /// Modify values
        /// </summary>
        Dictionary<string, dynamic> modifyValues = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets whether has value changed
        /// </summary>
        public bool HasValueChanged
        {
            get
            {
                return !(modifyValues?.IsNullOrEmpty() ?? true);
            }
        }

        /// <summary>
        /// Merge data
        /// </summary>
        /// <param name="data">New data</param>
        /// <returns>Return the warehouse data</returns>
        public TEntity MergeFromDataSource(TEntity data, IQuery newQuery)
        {
            if (data == null)
            {
                return WarehouseData;
            }
            ChangeLifeSource(DataLifeSource.DataSource);
            MergeData(data, newQuery);
            if (Operate == WarehouseDataOperate.Remove)
            {
                return default;
            }
            return WarehouseData;
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
            Operate = WarehouseDataOperate.Save;
            WarehouseData = data;
            if (IsRealRemove)
            {
                //add value
                LifeSource = DataLifeSource.New;
                IsRealRemove = false;
            }
            else
            {
                ComparisonData();
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        public void Remove()
        {
            Operate = WarehouseDataOperate.Remove;
        }

        /// <summary>
        /// Real remove
        /// </summary>
        public void RealRemove()
        {
            Remove();
            IsRealRemove = true;
        }

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modify">Modify expression</param>
        public void Modify(IModify modify)
        {
            if (modify == null)
            {
                return;
            }
            if (Operate == WarehouseDataOperate.Remove)
            {
                return;
            }
            if (WarehouseData != null)
            {
                ModifyData(WarehouseData, modify);
            }
            // modify data source value
            if (LifeSource == DataLifeSource.DataSource) 
            {
                if (PersistentData != null)
                {
                    ModifyData(PersistentData, modify);
                }
                ComparisonData();
            }
        }

        /// <summary>
        /// Change life source
        /// </summary>
        /// <param name="newLifeSource">New life source</param>
        public void ChangeLifeSource(DataLifeSource newLifeSource)
        {
            if (newLifeSource == LifeSource)
            {
                return;
            }
            LifeSource = newLifeSource;
            switch (newLifeSource)
            {
                case DataLifeSource.DataSource:
                    if (PersistentData == null)
                    {
                        PersistentData = WarehouseData.CopyOnlyWithIdentity();
                        ComparisonData();
                    }
                    break;
            }
        }

        /// <summary>
        ///  Get identity value is null or empty exception
        /// </summary>
        /// <returns></returns>
        EZNEWException IdentityValueIsNullOrEmptyException()
        {
            return new EZNEWException(string.Format("{0} identity value is null or empty", typeof(TEntity)));
        }

        /// <summary>
        /// Gets the new query fields
        /// </summary>
        /// <param name="newQuery">New query</param>
        /// <returns>Return query fields</returns>
        List<string> GetNewQueryFields(IQuery newQuery)
        {
            var newQueryFields = newQuery?.GetActuallyQueryFields(WarehouseData.GetType(), true) ?? new List<string>(0);
            var exceptFields = newQueryFields.Except(QueryFields).ToList();
            return exceptFields;
        }

        /// <summary>
        /// Merge data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="newQuery">New query</param>
        void MergeData(TEntity newData, IQuery newQuery)
        {
            if (newData == null)
            {
                return;
            }
            var newQueryFields = GetNewQueryFields(newQuery);
            if (newQueryFields.IsNullOrEmpty())
            {
                return;
            }
            //over new value
            foreach (var field in newQueryFields)
            {
                var newPropertyVal = newData.GetValue(field);
                PersistentData.SetValue(field, newPropertyVal);
                if (!modifyValues.ContainsKey(field))
                {
                    WarehouseData.SetValue(field, newPropertyVal);
                }
            }
            AddQueryFields(newQueryFields);
        }

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="modify">Modify expression</param>
        void ModifyData(TEntity data, IModify modify)
        {
            if (data == null || modify == null)
            {
                return;
            }
            var modifyValues = modify.GetModifyValues() ?? new Dictionary<string, IModifyValue>(0);
            if (modifyValues.Count < 1)
            {
                return;
            }
            foreach (var mv in modifyValues)
            {
                var nowValue = data.GetValue(mv.Key);
                var modifyValue = mv.Value;
                var newValue = modifyValue.GetModifyValue(nowValue);
                data.SetValue(mv.Key, newValue);
            }
        }

        /// <summary>
        /// Comparison data
        /// </summary>
        void ComparisonData()
        {
            if (PersistentData == null)
            {
                return;
            }
            modifyValues?.Clear();
            var newValues = WarehouseData.GetAllValues();
            var nowValues = PersistentData.GetAllValues();
            foreach (var newItem in newValues)
            {
                if (!nowValues.ContainsKey(newItem.Key))
                {
                    modifyValues.Add(newItem.Key, newItem.Value);
                    continue;
                }
                var nowValue = nowValues[newItem.Key];
                if (newItem.Value != nowValue)
                {
                    modifyValues.Add(newItem.Key, newItem.Value);
                }
            }
        }

        /// <summary>
        /// Add query fields
        /// </summary>
        /// <param name="fields">New query fields</param>
        void AddQueryFields(IEnumerable<string> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return;
            }
            QueryFields.AddRange(fields);
        }

        /// <summary>
        /// Create new data
        /// </summary>
        /// <param name="data">Entity data</param>
        /// <returns></returns>
        public static DataPackage<TEntity> CreateNewDataPackage(TEntity data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var dataPackage = new DataPackage<TEntity>()
            {
                WarehouseData = data.Copy(),
                LifeSource = DataLifeSource.New,
                Operate = WarehouseDataOperate.Save
            };
            dataPackage.AddQueryFields(EntityManager.GetPrimaryKeys(typeof(TEntity)));
            return dataPackage;
        }

        /// <summary>
        /// Create persistent data package
        /// </summary>
        /// <param name="data">Entity data</param>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data package</returns>
        public static DataPackage<TEntity> CreatePersistentDataPackage(TEntity data, IQuery query = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var copyData = data.Copy();
            var dataPackage = new DataPackage<TEntity>()
            {
                WarehouseData = copyData,
                PersistentData = copyData,
                LifeSource = DataLifeSource.DataSource,
                Operate = WarehouseDataOperate.None
            };
            dataPackage.AddQueryFields(query?.GetActuallyQueryFields(data.GetType(), true) ?? Array.Empty<string>());
            return dataPackage;
        }
    }
}
