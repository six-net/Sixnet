using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.Entity;
using EZNEW.Exceptions;
using EZNEW.Model;

namespace EZNEW.Development.Domain.Repository.Warehouse.Storage
{
    /// <summary>
    /// Defines entity package
    /// </summary>
    public class EntityPackage<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        internal EntityPackage() { }

        #region Properties

        /// <summary>
        /// Gets or sets the latest data
        /// </summary>
        public TEntity LatestData { get; set; }

        /// <summary>
        /// Gets or sets the original data
        /// </summary>
        public TEntity OriginalData { get; set; }

        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        public DataRecordOperation Operation { get; set; } = DataRecordOperation.None;

        /// <summary>
        /// Indicates whether is remove by condition
        /// </summary>
        public bool IsRealRemove { get; private set; } = false;

        /// <summary>
        /// Gets or sets the entity source
        /// </summary>
        public DataSource Source { get; private set; } = DataSource.Storage;

        /// <summary>
        /// Gets or sets the query fields
        /// </summary>
        public HashSet<string> QueryFields { get; set; } = new HashSet<string>();

        /// <summary>
        /// Modify values
        /// </summary>
        readonly Dictionary<string, dynamic> ModificationValues = new Dictionary<string, dynamic>();

        /// <summary>
        /// Indicates whether is a complete entity
        /// </summary>
        public bool CompleteEntity { get; set; }

        /// <summary>
        /// Gets whether has value changed
        /// </summary>
        public bool HasValueChanged => !(ModificationValues?.IsNullOrEmpty() ?? true);

        #endregion

        #region Methods

        /// <summary>
        /// Merge entity data from storage
        /// </summary>
        /// <param name="storageEntity">Entity data</param>
        /// <returns>Return the warehouse data</returns>
        public TEntity MergeStorageData(TEntity storageEntity, IQuery newQuery)
        {
            if (storageEntity == null)
            {
                return LatestData;
            }
            ChangeDataSource(DataSource.Storage);
            MergeData(storageEntity, newQuery);
            if (Operation == DataRecordOperation.Remove)
            {
                return default;
            }
            return LatestData;
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public Result Save(TEntity entity)
        {
            if (entity == null)
            {
                return Result.FailedResult("Entity is null");
            }
            Operation = DataRecordOperation.Save;
            LatestData = entity;
            if (IsRealRemove)
            {
                //add value
                Source = DataSource.New;
                IsRealRemove = false;
            }
            else
            {
                CompareData();
            }
            return Result.SuccessResult();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="force">Indicates whether force remove</param>
        /// <returns></returns>
        public Result Remove(bool force = false)
        {
            Operation = Operation == DataRecordOperation.Save && Source == DataSource.New ? DataRecordOperation.None : DataRecordOperation.Remove;
            if (force)
            {
                IsRealRemove = true;
            }
            return Result.SuccessResult();
        }

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyication">Modification expression</param>
        public void Modify(IModification modifyication)
        {
            if (modifyication == null || Operation == DataRecordOperation.Remove)
            {
                return;
            }
            if (LatestData != null)
            {
                ModifyData(LatestData, modifyication);
            }
            // modify storage value
            if (Source == DataSource.Storage)
            {
                if (OriginalData != null)
                {
                    ModifyData(OriginalData, modifyication);
                }
                CompareData();
            }
        }

        /// <summary>
        /// Change data source
        /// </summary>
        /// <param name="newSource">New source</param>
        public void ChangeDataSource(DataSource newSource)
        {
            if (newSource == Source)
            {
                return;
            }
            Source = newSource;
            switch (newSource)
            {
                case DataSource.Storage:
                    if (OriginalData == null)
                    {
                        OriginalData = LatestData.CloneOnlyWithIdentity();
                        CompareData();
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
            var newQueryFields = newQuery?.GetActuallyQueryFields(LatestData.GetType(), true) ?? new List<string>(0);
            var exceptFields = newQueryFields.Except(QueryFields).ToList();
            return exceptFields;
        }

        /// <summary>
        /// Merge data
        /// </summary>
        /// <param name="newEntity">New data</param>
        /// <param name="newQuery">New query</param>
        void MergeData(TEntity newEntity, IQuery newQuery)
        {
            if (newEntity == null)
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
                var newPropertyVal = newEntity.GetValue(field);
                OriginalData.SetValue(field, newPropertyVal);
                if (!ModificationValues.ContainsKey(field))
                {
                    LatestData.SetValue(field, newPropertyVal);
                }
            }
            AddQueryField(newQueryFields);
        }

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="entity">Data</param>
        /// <param name="modification">Modify expression</param>
        void ModifyData(TEntity entity, IModification modification)
        {
            if (entity == null || modification == null)
            {
                return;
            }
            var modificationEntries = modification.ModificationEntries;
            if (modificationEntries.IsNullOrEmpty())
            {
                return;
            }
            foreach (var modificationEntry in modificationEntries)
            {
                var nowValue = entity.GetValue(modificationEntry.FieldName);
                var modificationValue = modificationEntry.Value;
                var newValue = modificationValue.GetModifiedValue(nowValue);
                entity.SetValue(modificationEntry.FieldName, newValue);
            }
        }

        /// <summary>
        /// Compage data
        /// </summary>
        void CompareData()
        {
            if (OriginalData == null)
            {
                return;
            }
            ModificationValues?.Clear();
            var newValues = LatestData.GetAllValues();
            var nowValues = OriginalData.GetAllValues();
            foreach (var newItem in newValues)
            {
                if (!nowValues.ContainsKey(newItem.Key))
                {
                    ModificationValues.Add(newItem.Key, newItem.Value);
                    continue;
                }
                var nowValue = nowValues[newItem.Key];
                if (newItem.Value != nowValue)
                {
                    ModificationValues.Add(newItem.Key, newItem.Value);
                }
            }
        }

        /// <summary>
        /// Add query field
        /// </summary>
        /// <param name="fields">New query fields</param>
        void AddQueryField(IEnumerable<string> fields)
        {
            if (!fields.IsNullOrEmpty())
            {
                QueryFields.UnionWith(fields);
                CompleteEntity = QueryFields.Count == EntityManager.GetQueryFields<TEntity>().GetCount();
            }
        }

        /// <summary>
        /// Create new data package
        /// </summary>
        /// <param name="entity">Entity data</param>
        /// <returns></returns>
        public static EntityPackage<TEntity> CreateNewDataPackage(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var dataPackage = new EntityPackage<TEntity>()
            {
                LatestData = entity.Clone(),
                Source = DataSource.New,
                Operation = DataRecordOperation.Save
            };
            dataPackage.AddQueryField(EntityManager.GetPrimaryKeys(typeof(TEntity)));
            return dataPackage;
        }

        /// <summary>
        /// Create persistent data package
        /// </summary>
        /// <param name="entity">Entity data</param>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data package</returns>
        public static EntityPackage<TEntity> CreateStorageDataPackage(TEntity entity, IQuery query = null)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            var dataPackage = new EntityPackage<TEntity>()
            {
                LatestData = entity,
                OriginalData = entity.Clone(),
                Source = DataSource.Storage,
                Operation = DataRecordOperation.None
            };
            dataPackage.AddQueryField(query?.GetActuallyQueryFields(entity.GetType(), true) ?? Array.Empty<string>());
            return dataPackage;
        }

        #endregion
    }
}
