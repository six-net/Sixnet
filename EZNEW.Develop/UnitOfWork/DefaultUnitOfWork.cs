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
        //command list
        List<ICommand> commandList = null;
        Dictionary<string, Tuple<ICommandEngine, List<ICommand>>> commandGroup = null;
        int allowEmptyResultCommandCount = 0;

        //records
        List<IActivationRecord> activationRecords = new List<IActivationRecord>();
        Dictionary<int, IActivationRecord> activationRecordValueCollection = new Dictionary<int, IActivationRecord>();
        Dictionary<string, int> activationRecordCollection = new Dictionary<string, int>();
        Dictionary<Guid, IDataWarehouse> repositoryWarehouses = new Dictionary<Guid, IDataWarehouse>();//data warehouse
        int commandCounter = 1;
        int recordCounter = 1;

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
        void ResolveActivationRecord()
        {
            foreach (var record in activationRecords)
            {
                if (record == null)
                {
                    continue;
                }
                ResolveSingleActivationRecord(record);
            }
        }

        /// <summary>
        /// resolve single activation record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="records"></param>
        void ResolveSingleActivationRecord(IActivationRecord record)
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
                    if (activationRecordCollection.TryGetValue(record.RecordIdentity, out int maxRecordId))
                    {
                        if (record.Id >= maxRecordId)
                        {
                            activationRecordCollection[record.RecordIdentity] = record.Id;
                        }
                    }
                    else
                    {
                        activationRecordCollection.Add(record.RecordIdentity, record.Id);
                    }
                    activationRecordValueCollection[record.Id] = record;
                    break;
            }
            if (followRecords.IsNullOrEmpty())
            {
                return;
            }
            foreach (var followRecord in followRecords)
            {
                ResolveSingleActivationRecord(followRecord);
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
                if (commandGroup.IsNullOrEmpty())
                {
                    return new CommitResult()
                    {
                        CommitCommandCount = 0,
                        ExecutedDataCount = 0
                    };
                }
                bool beforeExecuteResult = await ExecuteCommandBeforeExecuteAsync().ConfigureAwait(false);
                if (!beforeExecuteResult)
                {
                    throw new Exception("any command BeforeExecute event return fail");
                }
                var result = await CommandExecuteManager.ExecuteAsync(commandGroup.Values).ConfigureAwait(false);
                var commitResult = new CommitResult()
                {
                    CommitCommandCount = commandList.Count,
                    ExecutedDataCount = result,
                    AllowEmptyResultCommandCount = allowEmptyResultCommandCount
                };
                await ExecuteCommandCallbackAsync(commitResult.EmptyResultOrSuccess).ConfigureAwait(false);
                if (commitResult.EmptyResultOrSuccess)
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
            ResolveActivationRecord();//resolve records
            var recordIds = activationRecordCollection.Values.OrderBy(c => c);
            commandGroup = new Dictionary<string, Tuple<ICommandEngine, List<ICommand>>>(activationRecordCollection.Count);
            commandList = new List<ICommand>(activationRecordCollection.Count);
            allowEmptyResultCommandCount = 0;
            foreach (var recordId in recordIds)
            {
                if (activationRecordValueCollection.TryGetValue(recordId, out var record) && record != null)
                {
                    var command = record.GetExecuteCommand();
                    if (command?.IsObsolete ?? true)
                    {
                        continue;
                    }
                    commandList.Add(command);
                    if (command.MustReturnValueOnSuccess)
                    {
                        allowEmptyResultCommandCount += 1;
                    }
                    var commandEngines = CommandExecuteManager.GetCommandEngines(command);
                    if (commandEngines.IsNullOrEmpty())
                    {
                        continue;
                    }
                    foreach (var engine in commandEngines)
                    {
                        if (engine == null)
                        {
                            continue;
                        }
                        var engineKey = engine.IdentityKey;
                        if (!commandGroup.TryGetValue(engineKey, out Tuple<ICommandEngine, List<ICommand>> engineValues))
                        {
                            engineValues = new Tuple<ICommandEngine, List<ICommand>>(engine, new List<ICommand>(activationRecordValueCollection.Count));
                        }
                        engineValues.Item2.Add(command);
                        commandGroup[engineKey] = engineValues;
                    }
                }
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
            if (!repositoryWarehouses.TryGetValue(entityTypeId, out var warehouse))
            {
                warehouse = new DataWarehouse<ET>();
                repositoryWarehouses.Add(entityTypeId, warehouse);
            }
            return warehouse as DataWarehouse<ET>;
        }

        /// <summary>
        /// commit completed
        /// </summary>
        void CommitCompleted()
        {
            commandCounter = 0;
            recordCounter = 0;
            allowEmptyResultCommandCount = 0;
            commandList?.Clear();
            commandGroup?.Clear();
            activationRecords?.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            CommitCompleted();
            repositoryWarehouses?.Clear();
        }

        /// <summary>
        /// Execute Command Before Execute
        /// </summary>
        /// <param name="cmds">command</param>
        async Task<bool> ExecuteCommandBeforeExecuteAsync()
        {
            if (commandList.IsNullOrEmpty())
            {
                return false;
            }
            bool result = true;
            foreach (var cmd in commandList)
            {
                result = result && await cmd.ExecuteBeforeAsync().ConfigureAwait(false);
            }
            return result;
        }

        /// <summary>
        /// Execute Command Callback
        /// </summary>
        /// <param name="cmds">commands</param>
        async Task ExecuteCommandCallbackAsync(bool success)
        {
            if (commandList.IsNullOrEmpty())
            {
                return;
            }
            foreach (var cmd in commandList)
            {
                await cmd.ExecuteCompleteAsync(success).ConfigureAwait(false);
            }
        }
    }
}
