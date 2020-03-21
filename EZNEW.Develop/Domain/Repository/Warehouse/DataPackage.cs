using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// data package
    /// </summary>
    public class DataPackage<T> where T : BaseEntity<T>,new()
    {
        internal DataPackage()
        {

        }

        /// <summary>
        /// warehouse data
        /// </summary>
        public T WarehouseData
        {
            get; set;
        }

        /// <summary>
        /// stored data
        /// </summary>
        public T PersistentData
        {
            get; set;
        }

        /// <summary>
        /// operate
        /// </summary>
        public WarehouseDataOperate Operate
        {
            get; set;
        } = WarehouseDataOperate.None;

        /// <summary>
        /// remove by condition
        /// </summary>
        public bool IsRealRemove { get; private set; } = false;

        /// <summary>
        /// stored from
        /// </summary>
        public DataLifeSource LifeSource
        {
            get; private set;
        } = DataLifeSource.DataSource;

        /// <summary>
        /// query fields
        /// </summary>
        public List<string> QueryFields { get; set; } = new List<string>();

        /// <summary>
        /// modify values
        /// </summary>
        Dictionary<string, dynamic> modifyValues = new Dictionary<string, dynamic>();

        /// <summary>
        /// has value changed
        /// </summary>
        public bool HasValueChanged
        {
            get
            {
                return !(modifyValues?.IsNullOrEmpty() ?? true);
            }
        }

        /// <summary>
        /// merge data
        /// </summary>
        /// <param name="data">new data</param>
        /// <returns></returns>
        public T MergeFromDataSource(T data, IQuery newQuery)
        {
            if (data == null)
            {
                return WarehouseData;
            }
            ChangeLifeSource(DataLifeSource.DataSource);
            MergeData(data, newQuery);
            if (Operate == WarehouseDataOperate.Remove)
            {
                return default(T);
            }
            return WarehouseData;
        }

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data"></param>
        public void Save(T data)
        {
            if (data == null)
            {
                return;
            }
            Operate = WarehouseDataOperate.Save;
            WarehouseData = data;
            if (IsRealRemove) //add value
            {
                LifeSource = DataLifeSource.New;
                IsRealRemove = false;
            }
            else
            {
                ComparisonData();
            }
        }

        /// <summary>
        /// remove
        /// </summary>
        public void Remove()
        {
            Operate = WarehouseDataOperate.Remove;
        }

        /// <summary>
        /// real remove
        /// </summary>
        public void RealRemove()
        {
            Remove();
            IsRealRemove = true;
        }

        /// <summary>
        /// modify
        /// </summary>
        /// <param name="modify">modify expression</param>
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
            if (LifeSource == DataLifeSource.DataSource) // modify data source value
            {
                if (PersistentData != null)
                {
                    ModifyData(PersistentData, modify);
                }
                ComparisonData();
            }
        }

        /// <summary>
        /// change life source
        /// </summary>
        /// <param name="newLifeSource">new life source</param>
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
        ///  get identity value is null or empty exception
        /// </summary>
        /// <returns></returns>
        EZNEWException IdentityValueIsNullOrEmptyException()
        {
            return new EZNEWException(string.Format("{0} identity value is null or empty", typeof(T)));
        }

        /// <summary>
        /// get new query fields
        /// </summary>
        /// <param name="newQuery">new query</param>
        /// <returns></returns>
        List<string> GetNewQueryFields(IQuery newQuery)
        {
            var newQueryFields = newQuery?.GetActuallyQueryFields(WarehouseData.GetType())?.Select(c => c.PropertyName) ?? new List<string>(0);
            var exceptFields = newQueryFields.Except(QueryFields).ToList();
            return exceptFields;
        }

        /// <summary>
        /// merge data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="newQuery"></param>
        void MergeData(T newData, IQuery newQuery)
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
            var modifyPropertyNames = modifyValues?.Keys.ToList() ?? new List<string>(0);
            foreach (var field in newQueryFields)
            {
                var newPropertyVal = newData.GetPropertyValue(field);
                PersistentData.SetPropertyValue(field, newPropertyVal);
                if (!modifyPropertyNames.Contains(field))
                {
                    WarehouseData.SetPropertyValue(field, newPropertyVal);
                }
            }
            AddQueryFields(newQueryFields);
        }

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="data"></param>
        void ModifyData(T data, IModify modify)
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
                var nowValue = data.GetPropertyValue(mv.Key);
                var modifyValue = mv.Value;
                var newValue = modifyValue.GetModifyValue(nowValue);
                data.SetPropertyValue(mv.Key, newValue);
            }
        }

        /// <summary>
        /// comparison data
        /// </summary>
        void ComparisonData()
        {
            if (PersistentData == null)
            {
                return;
            }
            modifyValues?.Clear();
            var newValues = WarehouseData.GetAllPropertyValues();
            var nowValues = PersistentData.GetAllPropertyValues();
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
        /// add query fields
        /// </summary>
        /// <param name="fields"></param>
        void AddQueryFields(IEnumerable<string> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return;
            }
            QueryFields.AddRange(fields);
        }

        /// <summary>
        /// create new data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataPackage<T> CreateNewDataPackage(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var dataPackage = new DataPackage<T>()
            {
                WarehouseData = data.Copy(),
                LifeSource = DataLifeSource.New,
                Operate = WarehouseDataOperate.Save
            };
            dataPackage.AddQueryFields(EntityManager.GetPrimaryKeys(typeof(T)).Select(c => c.PropertyName));
            return dataPackage;
        }

        /// <summary>
        /// create persistent data package
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataPackage<T> CreatePersistentDataPackage(T data, IQuery query = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            var copyData = data.Copy();
            var dataPackage = new DataPackage<T>()
            {
                WarehouseData = copyData,
                PersistentData = copyData,
                LifeSource = DataLifeSource.DataSource,
                Operate = WarehouseDataOperate.None
            };
            dataPackage.AddQueryFields(query?.GetActuallyQueryFields(data.GetType())?.Select(c => c.PropertyName));
            return dataPackage;
        }
    }
}
