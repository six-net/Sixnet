using EZNEW.Develop.Command;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Event;
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
        /// <summary>
        /// instance a defaultunitofwork object
        /// </summary>
        internal DefaultUnitOfWork()
        {
            WorkFactory.Current?.Dispose();
            WorkId = Guid.NewGuid().ToString();
            DomainEventManager = new DomainEventManager();
            WorkFactory.InvokeCreateWorkEventHandler(this);
            WorkFactory.Current = this;
        }

        #region fields

        //command list
        List<ICommand> commandList = null;
        Dictionary<string, Tuple<ICommandEngine, List<ICommand>>> commandGroup = null;
        int allowEmptyResultCommandCount = 0;
        int commandCounter = 1;

        //records
        List<IActivationRecord> activationRecords = new List<IActivationRecord>();
        Dictionary<int, IActivationRecord> activationRecordValueCollection = new Dictionary<int, IActivationRecord>();
        Dictionary<string, int> activationRecordCollection = new Dictionary<string, int>();
        int recordCounter = 1;

        //data warehouses
        Dictionary<Guid, IDataWarehouse> repositoryWarehouses = new Dictionary<Guid, IDataWarehouse>();//data warehouse 

        //event handler
        static event Action<IUnitOfWork, CommitResult, IEnumerable<ICommand>> commitSuccessEventHandler;

        #endregion

        #region propertys

        /// <summary>
        /// domain event manager
        /// </summary>
        public DomainEventManager DomainEventManager { get; private set; }

        /// <summary>
        /// domain events
        /// </summary>
        public List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

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
        /// data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region methods

        #region activation record

        #region add work activation

        /// <summary>
        /// add work activation
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

        #endregion

        #region resolve activation record

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

        #endregion

        /// <summary>
        /// get record id
        /// </summary>
        /// <returns></returns>
        public int GetRecordId()
        {
            return recordCounter++;
        }

        #endregion

        #region command

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
                    if (!command.MustReturnValueOnSuccess)
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
        /// get command id
        /// </summary>
        /// <returns></returns>
        public int GetCommandId()
        {
            return commandCounter++;
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
                result = result && (cmd.ExecuteBeforeExecuteOperation()?.AllowExecuteCommand ?? false);
            }
            return await Task.FromResult(result);
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
                cmd.ExecuteCallbackOperation(success);
            }
            await Task.CompletedTask;
        }

        #endregion

        #region commit

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
                var executeOption = GetCommandExecuteOption();
                var result = await CommandExecuteManager.ExecuteAsync(executeOption, commandGroup.Values).ConfigureAwait(false);
                var commitResult = new CommitResult()
                {
                    CommitCommandCount = commandList.Count,
                    ExecutedDataCount = result,
                    AllowEmptyResultCommandCount = allowEmptyResultCommandCount
                };
                await ExecuteCommandCallbackAsync(commitResult.EmptyResultOrSuccess).ConfigureAwait(false);
                if (commitResult.EmptyResultOrSuccess)
                {
                    InvokeCommitSuccessEventHandler(commitResult);//local unit work success callback
                    WorkFactory.InvokeWorkCommitSuccessEventHandler(this, commitResult, commandList);//unit work global success callback
                    await ExecuteWorkCompletedDomainEventAsync().ConfigureAwait(false);//execute domain event
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
        /// get command execute option
        /// </summary>
        /// <returns></returns>
        CommandExecuteOption GetCommandExecuteOption()
        {
            if (!IsolationLevel.HasValue)
            {
                return CommandExecuteOption.Default;
            }
            return new CommandExecuteOption()
            {
                IsolationLevel = IsolationLevel
            };
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

        #endregion

        #region warehouse

        /// <summary>
        /// get data warehouse by entity type
        /// </summary>
        /// <returns></returns>
        public DataWarehouse<ET> GetWarehouse<ET>() where ET : BaseEntity<ET>, new()
        {
            var entityTypeId = typeof(ET).GUID;
            if (!repositoryWarehouses.TryGetValue(entityTypeId, out var warehouse))
            {
                warehouse = new DataWarehouse<ET>();
                repositoryWarehouses.Add(entityTypeId, warehouse);
            }
            return warehouse as DataWarehouse<ET>;
        }

        #endregion

        #region work event

        #region register commit success event handler

        /// <summary>
        /// register commit success event handler
        /// </summary>
        /// <param name="handlers">handlers</param>
        public void RegisterCommitSuccessEventHandler(params Action<IUnitOfWork, CommitResult, IEnumerable<ICommand>>[] handlers)
        {
            if (handlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in handlers)
            {
                commitSuccessEventHandler += handler;
            }
        }

        #endregion

        #region invoke commit success event handler

        /// <summary>
        /// invoke commit success event handler
        /// </summary>
        /// <param name="commitResult"></param>
        void InvokeCommitSuccessEventHandler(CommitResult commitResult)
        {
            commitSuccessEventHandler?.Invoke(this, commitResult, commandList);
        }

        #endregion

        #region 

        #endregion

        #endregion

        #region domain event

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        public async Task PublishDomainEventAsync(params IDomainEvent[] domainEvents)
        {
            if (domainEvents.IsNullOrEmpty())
            {
                return;
            }
            DomainEvents.AddRange(domainEvents);
            await DomainEventManager.PublishAsync(domainEvents).ConfigureAwait(false);
        }

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        public void PublishDomainEvent(params IDomainEvent[] domainEvents)
        {
            PublishDomainEventAsync(domainEvents).Wait();
        }

        /// <summary>
        /// execute work completed domain event
        /// </summary>
        async Task ExecuteWorkCompletedDomainEventAsync()
        {
            var eventArray = DomainEvents.ToArray();
            if (DomainEventBus.globalDomainEventManager != null)
            {
                await DomainEventBus.globalDomainEventManager.ExecutedTimeDomainEventAsync(EventTriggerTime.WorkCompleted, eventArray).ConfigureAwait(false);//execute global event handler
            }
            await DomainEventManager.ExecutedTimeDomainEventAsync(EventTriggerTime.WorkCompleted, eventArray).ConfigureAwait(false);//execute local work event handler
        }

        #endregion

        #region dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            CommitCompleted();
            repositoryWarehouses?.Clear();
            DomainEventManager = null;
            DomainEvents?.Clear();
        }

        #endregion

        #endregion
    }
}
