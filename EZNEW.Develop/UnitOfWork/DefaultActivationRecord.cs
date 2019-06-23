using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using EZNEW.Framework.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// default activation record
    /// </summary>
    /// <typeparam name="ET"></typeparam>
    public class DefaultActivationRecord<ET, DAI> : IActivationRecord where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id
        {
            get; set;
        }

        /// <summary>
        /// parent record
        /// </summary>
        public IActivationRecord ParentRecord { get; set; }

        /// <summary>
        /// operation
        /// </summary>
        public ActivationOperation Operation
        {
            get; set;
        }

        /// <summary>
        /// identity value
        /// </summary>
        public string IdentityValue
        {
            get; set;
        }

        /// <summary>
        /// query
        /// </summary>
        public IQuery Query
        {
            get; set;
        }

        /// <summary>
        /// modify expression
        /// </summary>
        public IModify ModifyExpression
        {
            get; set;
        }

        /// <summary>
        /// Follow Records
        /// </summary>
        private List<IActivationRecord> FollowRecords
        {
            get; set;
        }

        /// <summary>
        /// data access service
        /// </summary>
        public Type DataAccessService
        {
            get; set;
        }

        /// <summary>
        /// entity type
        /// </summary>
        public Type EntityType
        {
            get; set;
        }

        /// <summary>
        /// is obsolete
        /// </summary>
        public bool IsObsolete { get; private set; }

        /// <summary>
        /// record identity
        /// </summary>
        public string RecordIdentity
        {
            get; private set;
        }

        private DefaultActivationRecord()
        { }

        /// <summary>
        /// create operation
        /// </summary>
        /// <param name="operation">operation</param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateRecord(ActivationOperation operation, string identityValue)
        {
            var record = new DefaultActivationRecord<ET, DAI>()
            {
                Operation = operation,
                IdentityValue = identityValue,
                DataAccessService = typeof(DAI),
                EntityType = typeof(ET)
            };
            switch (operation)
            {
                case ActivationOperation.SaveObject:
                case ActivationOperation.RemoveByObject:
                    record.RecordIdentity =string.Format("{0}_{1}",typeof(ET).GUID,identityValue);
                    break;
                default:
                    record.RecordIdentity = Guid.NewGuid().ToString();
                    break;
            }
            return record;
        }

        /// <summary>
        /// create save operation
        /// </summary>
        /// <param name="identityValue">identity values</param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateSaveRecord(string identityValue)
        {
            if (identityValue.IsNullOrEmpty())
            {
                throw new EZNEWException("identityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.SaveObject, identityValue);
        }

        /// <summary>
        /// create remove object operation
        /// </summary>
        /// <param name="identityValue"></param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateRemoveObjectRecord(string identityValue)
        {
            if (identityValue.IsNullOrEmpty())
            {
                throw new EZNEWException("identityValue is null or empty");
            }
            return CreateRecord(ActivationOperation.RemoveByObject, identityValue);
        }

        /// <summary>
        /// create remove by condition record
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <typeparam name="DAI"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateRemoveByConditionRecord(IQuery query)
        {
            var record = CreateRecord(ActivationOperation.RemoveByCondition, null);
            record.Query = query;
            return record;
        }

        /// <summary>
        /// create modify record
        /// </summary>
        /// <param name="modify"></param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateModifyRecord(IModify modify, IQuery query)
        {
            var record = CreateRecord(ActivationOperation.ModifyByExpression, null);
            record.ModifyExpression = modify;
            record.Query = query;
            return record;
        }

        /// <summary>
        /// create remove record
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreateRemoveRecord(IQuery query)
        {
            var record = CreateRecord(ActivationOperation.ModifyByExpression, null);
            record.Query = query;
            return record;
        }

        /// <summary>
        /// create package record
        /// </summary>
        /// <returns></returns>
        public static DefaultActivationRecord<ET, DAI> CreatePackageRecord()
        {
            return CreateRecord(ActivationOperation.Package, string.Empty);
        }

        /// <summary>
        /// add follow records
        /// </summary>
        /// <param name="records">records</param>
        public void AddFollowRecords(params IActivationRecord[] records)
        {
            if (records.IsNullOrEmpty())
            {
                return;
            }
            if (FollowRecords == null)
            {
                FollowRecords = new List<IActivationRecord>();
            }
            foreach (var childRecord in records)
            {
                childRecord.ParentRecord = this;
                FollowRecords.Add(childRecord);
            }
        }

        /// <summary>
        /// get follow records
        /// </summary>
        public List<IActivationRecord> GetFollowRecords()
        {
            return FollowRecords ?? new List<IActivationRecord>(0);
        }

        /// <summary>
        /// get execute commands
        /// </summary>
        /// <returns></returns>
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
        /// get save commands
        /// </summary>
        /// <returns></returns>
        ICommand GetSaveObjectCommand()
        {
            if (IdentityValue.IsNullOrEmpty())
            {
                return null;
            }
            var dataPackage = WarehouseManager.GetDataPackage<ET>(IdentityValue);
            if (dataPackage == null || dataPackage.Operate != WarehouseDataOperate.Save)
            {
                return null;
            }
            var dalService = ContainerManager.Resolve<DAI>();
            if (dataPackage.LifeSource == DataLifeSource.New) //new add value
            {
                return dalService.Add((ET)dataPackage.WarehouseData);
            }
            else if (dataPackage.HasValueChanged) // update value
            {
                var data = (ET)dataPackage.WarehouseData;
                return dalService.Modify(data, (ET)dataPackage.PersistentData);
            }
            return null;
        }

        /// <summary>
        /// geet remove command
        /// </summary>
        /// <returns></returns>
        ICommand GetRemoveObjectCommand()
        {
            if (IdentityValue.IsNullOrEmpty())
            {
                return null;
            }
            var dataPackage = WarehouseManager.GetDataPackage<ET>(IdentityValue);
            if (dataPackage == null || dataPackage.Operate != WarehouseDataOperate.Remove || dataPackage.IsRealRemove)
            {
                return null;
            }
            var dalService = ContainerManager.Resolve<DAI>();
            var data = (ET)dataPackage.WarehouseData;
            return dalService.Delete(data);
        }

        /// <summary>
        /// get remove conditon command
        /// </summary>
        /// <returns></returns>
        ICommand GetRemoveConditionCommand()
        {
            var dalService = ContainerManager.Resolve<DAI>();
            return dalService.Delete(Query);
        }

        /// <summary>
        /// get modify expression command
        /// </summary>
        /// <returns></returns>
        ICommand GetModifyExpressionCommand()
        {
            var dalService = ContainerManager.Resolve<DAI>();
            return dalService.Modify(ModifyExpression, Query);
        }

        /// <summary>
        /// obsolete
        /// </summary>
        public void Obsolete()
        {
            IsObsolete = true;
        }
    }
}
