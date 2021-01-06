using System;
using System.Collections.Generic;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.DependencyInjection;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Default activation record
    /// </summary>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public class DefaultActivationRecord<TEntity, TDataAccess> : IActivationRecord where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        /// <summary>
        /// Gets or sets the record id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent record
        /// </summary>
        public IActivationRecord ParentRecord { get; set; }

        /// <summary>
        /// Gets or sets the activation operation
        /// </summary>
        public ActivationOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the identity value
        /// </summary>
        public string IdentityValue { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the modify expression
        /// </summary>
        public IModify ModifyExpression { get; set; }

        /// <summary>
        /// Gets or sets follow records
        /// </summary>
        private List<IActivationRecord> FollowRecords { get; set; }

        /// <summary>
        /// Gets or sets the data access service
        /// </summary>
        public Type DataAccessService { get; set; }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets whether the record is obsolete
        /// </summary>
        public bool IsObsolete { get; private set; }

        /// <summary>
        /// Gets the record identity
        /// </summary>
        public string RecordIdentity { get; private set; }

        /// <summary>
        /// Gets the activation options
        /// </summary>
        public ActivationOptions ActivationOptions { get; private set; }

        private DefaultActivationRecord() { }

        /// <summary>
        /// Create a activation record
        /// </summary>
        /// <param name="operation">Activation operation</param>
        /// <param name="identityValue">Object identity value</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateRecord(ActivationOperation operation, string identityValue, ActivationOptions activationOptions)
        {
            var record = new DefaultActivationRecord<TEntity, TDataAccess>()
            {
                Operation = operation,
                IdentityValue = identityValue,
                DataAccessService = typeof(TDataAccess),
                EntityType = typeof(TEntity)
            };
            activationOptions ??= ActivationOptions.Default;
            record.ActivationOptions = activationOptions;
            if (activationOptions.ForceExecute)
            {
                record.RecordIdentity = Guid.NewGuid().ToString();
            }
            else
            {
                switch (operation)
                {
                    case ActivationOperation.SaveObject:
                    case ActivationOperation.RemoveByObject:
                        record.RecordIdentity = string.Format("{0}_{1}", typeof(TEntity).GUID, identityValue);
                        break;
                    default:
                        record.RecordIdentity = Guid.NewGuid().ToString();
                        break;
                }
            }
            return record;
        }

        /// <summary>
        /// Create a save record
        /// </summary>
        /// <param name="identityValue">Identity values</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateSaveRecord(string identityValue, ActivationOptions activationOptions)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw new EZNEWException("IdentityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.SaveObject, identityValue, activationOptions);
        }

        /// <summary>
        /// Create remove object record
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateRemoveObjectRecord(string identityValue, ActivationOptions activationOptions)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw new EZNEWException("identityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.RemoveByObject, identityValue, activationOptions);
        }

        /// <summary>
        /// Create remove by condition record
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateRemoveByConditionRecord(IQuery query, ActivationOptions activationOptions)
        {
            var record = CreateRecord(ActivationOperation.RemoveByCondition, null, activationOptions);
            record.Query = query;
            return record;
        }

        /// <summary>
        /// Create modify record
        /// </summary>
        /// <param name="modify">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateModifyRecord(IModify modify, IQuery query, ActivationOptions activationOptions)
        {
            var record = CreateRecord(ActivationOperation.ModifyByExpression, null, activationOptions);
            record.ModifyExpression = modify;
            record.Query = query;
            return record;
        }

        /// <summary>
        /// Create package record
        /// </summary>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreatePackageRecord()
        {
            return CreateRecord(ActivationOperation.Package, string.Empty, null);
        }

        /// <summary>
        /// Add follow records
        /// </summary>
        /// <param name="records">Follow records</param>
        public void AddFollowRecords(params IActivationRecord[] records)
        {
            IEnumerable<IActivationRecord> recordCollection = records;
            AddFollowRecords(recordCollection);
        }

        /// <summary>
        /// Add follow records
        /// </summary>
        /// <param name="records">Follow records</param>
        public void AddFollowRecords(IEnumerable<IActivationRecord> records)
        {
            if (records.IsNullOrEmpty())
            {
                return;
            }
            FollowRecords ??= new List<IActivationRecord>(0);
            foreach (var childRecord in records)
            {
                childRecord.ParentRecord = this;
                FollowRecords.Add(childRecord);
            }
        }

        /// <summary>
        /// Get follow records
        /// </summary>
        public IEnumerable<IActivationRecord> GetFollowRecords()
        {
            return FollowRecords ?? new List<IActivationRecord>(0);
        }

        /// <summary>
        /// Get execute commands
        /// </summary>
        /// <returns>Return the record command</returns>
        public ICommand GetExecuteCommand()
        {
            ICommand command = null;
            switch (Operation)
            {
                case ActivationOperation.SaveObject:
                    command = GetSaveObjectCommand();
                    break;
                case ActivationOperation.RemoveByObject:
                    command = GetRemoveObjectCommand();
                    break;
                case ActivationOperation.RemoveByCondition:
                    command = GetRemoveConditionCommand();
                    break;
                case ActivationOperation.ModifyByExpression:
                    command = GetModifyExpressionCommand();
                    break;
            }
            return command;
        }

        /// <summary>
        /// Get save commands
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetSaveObjectCommand()
        {
            if (string.IsNullOrWhiteSpace(IdentityValue))
            {
                return null;
            }
            var dataPackage = WarehouseManager.GetDataPackage<TEntity>(IdentityValue);
            if (dataPackage == null || (!ActivationOptions.ForceExecute && dataPackage.Operate != WarehouseDataOperate.Save))
            {
                return null;
            }
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            if (dataPackage.LifeSource == DataLifeSource.New) //new add value
            {
                return dataAccessService.Add(dataPackage.WarehouseData);
            }
            else if (dataPackage.HasValueChanged) // update value
            {
                var data = dataPackage.WarehouseData;
                return dataAccessService.Modify(data, dataPackage.PersistentData);
            }
            return null;
        }

        /// <summary>
        /// Get remove command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetRemoveObjectCommand()
        {
            if (string.IsNullOrWhiteSpace(IdentityValue))
            {
                return null;
            }
            var dataPackage = WarehouseManager.GetDataPackage<TEntity>(IdentityValue);
            if (dataPackage == null || (!ActivationOptions.ForceExecute && (dataPackage.Operate != WarehouseDataOperate.Remove || dataPackage.IsRealRemove)))
            {
                return null;
            }
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            var data = dataPackage.WarehouseData;
            return dataAccessService.Delete(data);
        }

        /// <summary>
        /// Get remove conditon command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetRemoveConditionCommand()
        {
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            return dataAccessService.Delete(Query);
        }

        /// <summary>
        /// Get modify expression command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetModifyExpressionCommand()
        {
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            return dataAccessService.Modify(ModifyExpression, Query);
        }

        /// <summary>
        /// Obsolete record
        /// </summary>
        public void Obsolete()
        {
            IsObsolete = true;
        }
    }
}
