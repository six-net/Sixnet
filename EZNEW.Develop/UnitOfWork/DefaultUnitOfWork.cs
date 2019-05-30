using EZNEW.Develop.Command;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// default implements for IUnitOfWork
    /// </summary>
    public class DefaultUnitOfWork : IUnitOfWork
    {
        List<ICommand> commandList = new List<ICommand>();//command list
        List<IActivationRecord> activationRecords = new List<IActivationRecord>();
        ConcurrentDictionary<Guid, IDataWarehouse> repositoryWarehouses = new ConcurrentDictionary<Guid, IDataWarehouse>();//data warehouse
        int commandCounter = 0;
        int recordCounter = 0;

        /// <summary>
        /// commit success callback event
        /// </summary>
        public event Action CommitSuccessCallbackEvent;

        /// <summary>
        /// instance a defaultunitofwork object
        /// </summary>
        internal DefaultUnitOfWork()
        {
            WorkFactory.Current?.Dispose();
            WorkFactory.InvokeCreateWorkEvent();
            WorkFactory.Current = this;
            WorkId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// command count
        /// </summary>
        public int CommandCount
        {
            get
            {
                return commandList?.Count ?? 0;
            }
        }

        /// <summary>
        /// work id
        /// </summary>
        public string WorkId { get; } = string.Empty;

        /// <summary>
        /// Add Commands To UnitOfWork
        /// </summary>
        /// <param name="cmds">Commands</param>
        public void AddCommand(params ICommand[] cmds)
        {
            if (cmds == null)
            {
                return;
            }
            commandList.AddRange(cmds);
        }

        /// <summary>
        /// add activation operation
        /// </summary>
        /// <param name="records"></param>
        public void AddActivation(params IActivationRecord[] records)
        {
            if (records.IsNullOrEmpty())
            {
                return;
            }
            activationRecords.AddRange(records);
        }

        /// <summary>
        /// resolve activation record
        /// </summary>
        /// <param name="records">records</param>
        List<IActivationRecord> ResolveActivationRecord()
        {
            var records = new List<IActivationRecord>();
            foreach (var record in activationRecords)
            {
                if (record == null)
                {
                    continue;
                }
                ResolveSingleActivationRecord(record, ref records);
            }
            return records;
        }

        /// <summary>
        /// resolve single activation record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="records"></param>
        void ResolveSingleActivationRecord(IActivationRecord record, ref List<IActivationRecord> records)
        {
            if (record == null)
            {
                return;
            }
            List<IActivationRecord> followRecords = null;
            switch (record.Operation)
            {
                case ActivationOperation.Package:
                    followRecords = record.GetFollowRecords();
                    break;
                default:
                    record.Id = GetRecordId();
                    records.Add(record);
                    break;
            }
            if (followRecords.IsNullOrEmpty())
            {
                return;
            }
            foreach (var followRecord in followRecords)
            {
                ResolveSingleActivationRecord(followRecord, ref records);
            }
        }

        /// <summary>
        /// Commit Work
        /// </summary>
        /// <returns></returns>
        public CommitResult Commit()
        {
            return CommitAsync().Result;
        }

        /// <summary>
        /// Commit Work
        /// </summary>
        /// <returns></returns>
        public async Task<CommitResult> CommitAsync()
        {
            try
            {
                //build commands
                BuildCommand();
                //object command
                if (commandList.Count <= 0)
                {
                    return new CommitResult()
                    {
                        CommitCommandCount = 0,
                        ExecutedDataCount = 0
                    };
                }
                var exectCommandList = commandList.Where(c => !c.IsObsolete).ToList();
                bool beforeExecuteResult = await ExecuteCommandBeforeExecuteAsync(exectCommandList).ConfigureAwait(false);
                if (!beforeExecuteResult)
                {
                    throw new Exception("any command BeforeExecute event return fail");
                }
                var result = await CommandExecuteManager.ExecuteAsync(exectCommandList).ConfigureAwait(false);
                await ExecuteCommandCallbackAsync(exectCommandList, result > 0).ConfigureAwait(false);
                var commitResult = new CommitResult()
                {
                    CommitCommandCount = exectCommandList.Count,
                    ExecutedDataCount = result,
                    AllowNoneResultCommandCount = exectCommandList.Count(c => c.VerifyResult?.Invoke(0) ?? false)
                };
                if (commitResult.NoneCommandOrSuccess)
                {
                    CommitSuccessCallbackEvent?.Invoke();
                    WorkFactory.InvokeCommitSuccessEvent();
                }
                return commitResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CommitCompleted();
            }
        }

        /// <summary>
        /// parse activation record
        /// </summary>
        void BuildCommand()
        {
            var records = ResolveActivationRecord().OrderBy(r => r.Id);
            foreach (var record in records)
            {
                switch (record.Operation)
                {
                    case ActivationOperation.SaveObject:
                    case ActivationOperation.RemoveByObject:
                        if (activationRecords.Any(c => c.Id > record.Id && c.IdentityValue == record.IdentityValue))
                        {
                            continue;
                        }
                        break;
                }
                var commands = record.GetExecuteCommands();
                AddCommand(commands.ToArray());
            }
        }

        /// <summary>
        /// get record id
        /// </summary>
        /// <returns></returns>
        public int GetRecordId()
        {
            return recordCounter++;
        }

        /// <summary>
        /// get command id
        /// </summary>
        /// <returns></returns>
        public int GetCommandId()
        {
            return commandCounter++;
        }

        /// <summary>
        /// get data warehouse by entity type
        /// </summary>
        /// <returns></returns>
        public DataWarehouse<ET> GetWarehouse<ET>() where ET : BaseEntity<ET>
        {
            var entityTypeId = typeof(ET).GUID;
            var warehouse = repositoryWarehouses.GetOrAdd(entityTypeId, new DataWarehouse<ET>());
            return warehouse as DataWarehouse<ET>;
        }

        /// <summary>
        /// save warehouse
        /// </summary>
        /// <param name="warehouse">warehouse</param>
        public void SaveWarehouse<ET>(DataWarehouse<ET> warehouse) where ET : BaseEntity<ET>
        {
            if (warehouse == null)
            {
                return;
            }
            var entityTypeId = typeof(ET).GUID;
            repositoryWarehouses.AddOrUpdate(entityTypeId, warehouse, (eid, ow) =>
            {
                return warehouse;
            });
        }

        /// <summary>
        /// commit completed
        /// </summary>
        void CommitCompleted()
        {
            commandCounter = 0;
            recordCounter = 0;
            commandList.Clear();
            activationRecords.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            CommitCompleted();
            repositoryWarehouses.Clear();
        }

        /// <summary>
        /// Execute Command Before Execute
        /// </summary>
        /// <param name="cmds">command</param>
        static async Task<bool> ExecuteCommandBeforeExecuteAsync(IEnumerable<ICommand> cmds)
        {
            if (cmds == null)
            {
                return false;
            }
            bool result = true;
            foreach (var cmd in cmds)
            {
                result = result && await cmd.ExecuteBeforeAsync().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Execute Command Callback
        /// </summary>
        /// <param name="cmds">commands</param>
        static async Task ExecuteCommandCallbackAsync(IEnumerable<ICommand> cmds, bool success)
        {
            foreach (var cmd in cmds)
            {
                await cmd.ExecuteCompleteAsync(success).ConfigureAwait(false);
            }
        }

    }
}
