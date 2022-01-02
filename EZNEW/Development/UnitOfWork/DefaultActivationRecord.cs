using System;
using System.Collections.Generic;
using EZNEW.Development.Command;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Entity;
using EZNEW.Exceptions;
using EZNEW.DependencyInjection;
using EZNEW.Development.Domain.Repository.Warehouse.Storage;
using EZNEW.Logging;

namespace EZNEW.Development.UnitOfWork
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
        /// Gets or sets the modification expression
        /// </summary>
        public IModification ModificationExpression { get; set; }

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
            if (activationOptions.ForceExecution)
            {
                record.RecordIdentity = Guid.NewGuid().ToString();
            }
            else
            {
                switch (operation)
                {
                    case ActivationOperation.SaveObject:
                    case ActivationOperation.RemoveObject:
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
        /// Create a saving record
        /// </summary>
        /// <param name="identityValue">Identity values</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateSavingRecord(string identityValue, ActivationOptions activationOptions)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw new EZNEWException("IdentityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.SaveObject, identityValue, activationOptions);
        }

        /// <summary>
        /// Create removing object record
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateRemovingObjectRecord(string identityValue, ActivationOptions activationOptions)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw new EZNEWException("identityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.RemoveObject, identityValue, activationOptions);
        }

        /// <summary>
        /// Create removing by condition record
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateRemovingByConditionRecord(IQuery query, ActivationOptions activationOptions)
        {
            var record = CreateRecord(ActivationOperation.RemoveByCondition, null, activationOptions);
            record.Query = query;
            return record;
        }

        /// <summary>
        /// Create modification record
        /// </summary>
        /// <param name="modificationExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return a new default activation record</returns>
        public static DefaultActivationRecord<TEntity, TDataAccess> CreateModificationRecord(IModification modificationExpression, IQuery query, ActivationOptions activationOptions)
        {
            var record = CreateRecord(ActivationOperation.ModifyByExpression, null, activationOptions);
            record.ModificationExpression = modificationExpression;
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
        public void AddFollowRecord(params IActivationRecord[] records)
        {
            IEnumerable<IActivationRecord> recordCollection = records;
            AddFollowRecord(recordCollection);
        }

        /// <summary>
        /// Add follow records
        /// </summary>
        /// <param name="records">Follow records</param>
        public void AddFollowRecord(IEnumerable<IActivationRecord> records)
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
        /// Get execution command
        /// </summary>
        /// <returns>Return the record command</returns>
        public ICommand GetExecutionCommand()
        {
            ICommand command = null;
            switch (Operation)
            {
                case ActivationOperation.SaveObject:
                    command = GetSavingObjectCommand();
                    break;
                case ActivationOperation.RemoveObject:
                    command = GetRemovingObjectCommand();
                    break;
                case ActivationOperation.RemoveByCondition:
                    command = GetRemovingByConditionCommand();
                    break;
                case ActivationOperation.ModifyByExpression:
                    command = GetModificationExpressionCommand();
                    break;
            }
            return command;
        }

        /// <summary>
        /// Get saving object command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetSavingObjectCommand()
        {
            LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateSavingCommand, "Generate saving command");
            if (string.IsNullOrWhiteSpace(IdentityValue))
            {
                LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateSavingCommand, "Identity value is null or empty");
                return null;
            }
            var dataPackage = EntityStorage<TEntity>.GetCurrentEntityStorage(true).GetDataPackage(IdentityValue);
            if (dataPackage == null || (!ActivationOptions.ForceExecution && dataPackage.Operation != DataRecordOperation.Save))
            {
                return null;
            }
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            if (dataPackage.Source == DataSource.New) //new add value
            {
                LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateSavingCommand, "Generate add command");
                return dataAccessService.Add(dataPackage.LatestData);
            }
            else if (dataPackage.HasValueChanged) //update value
            {
                LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateSavingCommand, "Generate modification command");
                return dataAccessService.Modify(dataPackage.LatestData, dataPackage.OriginalData);
            }
            LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateSavingCommand, "No saving command generated");
            return null;
        }

        /// <summary>
        /// Get removing object command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetRemovingObjectCommand()
        {
            LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateRemovingCommand, "Generate removing command");
            if (string.IsNullOrWhiteSpace(IdentityValue))
            {
                LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateRemovingCommand, "Identity value is null or empty");
                return null;
            }
            var dataPackage = EntityStorage<TEntity>.GetCurrentEntityStorage(true).GetDataPackage(IdentityValue);
            if (dataPackage == null || (!ActivationOptions.ForceExecution && (dataPackage.Operation != DataRecordOperation.Remove || dataPackage.IsRealRemove)))
            {
                LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateRemovingCommand, "Ignore removing record");
                return null;
            }
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            return dataAccessService.Delete(dataPackage.LatestData);
        }

        /// <summary>
        /// Get removint by conditon command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetRemovingByConditionCommand()
        {
            LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateRemovingByConditionCommand, "Generate removing by condition command");
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            return dataAccessService.Delete(Query);
        }

        /// <summary>
        /// Get modification expression command
        /// </summary>
        /// <returns>Return the record command</returns>
        ICommand GetModificationExpressionCommand()
        {
            LogManager.LogDebug<DefaultActivationRecord<TEntity, TDataAccess>>(FrameworkLogEvents.ActivationRecord.GenerateModificationExpressionCommand, "Generate modification expression command");
            var dataAccessService = ContainerManager.Resolve<TDataAccess>();
            return dataAccessService.Modify(ModificationExpression, Query);
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
