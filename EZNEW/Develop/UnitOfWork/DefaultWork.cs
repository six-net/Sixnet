using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.DataAccess.Event;
using EZNEW.Develop.Domain.Event;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Diagnostics;
using EZNEW.Logging;
using EZNEW.Serialize;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Default implements for work
    /// </summary>
    internal class DefaultWork : IWork
    {
        /// <summary>
        /// Determine whether allow output trace log
        /// </summary>
        bool allowTraceLog = false;

        /// <summary>
        /// Initialize default work
        /// </summary>
        internal DefaultWork()
        {
            WorkId = Guid.NewGuid().ToString();
            allowTraceLog = SwitchManager.ShouldTraceFramework(sw =>
            {
                allowTraceLog = SwitchManager.ShouldTraceFramework();
            });
            DomainEventManager = new DomainEventManager();
            WorkManager.TriggerCreateWorkEvent(this);
            WorkManager.Current = this;
        }

        #region Fields

        /// <summary>
        /// command list
        /// </summary>
        List<ICommand> commandCollection = null;

        /// <summary>
        /// command execute engine
        /// key:engine key
        /// </summary>
        Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>> commandEngineGroups = null;

        /// <summary>
        /// allow empty result command count
        /// </summary>
        int allowEmptyResultCommandCount = 0;

        /// <summary>
        /// command counter
        /// </summary>
        int commandCounter = 1;

        /// <summary>
        /// all activation records
        /// </summary>
        readonly List<IActivationRecord> activationRecordCollection = new List<IActivationRecord>();

        /// <summary>
        /// key:record id
        /// </summary>
        readonly Dictionary<int, IActivationRecord> activationRecordValues = new Dictionary<int, IActivationRecord>();

        /// <summary>
        /// same record identity max record dictionary
        /// key:record identity
        /// value:max record id
        /// </summary>
        readonly Dictionary<string, int> activationMaxRecordIds = new Dictionary<string, int>();

        /// <summary>
        /// activation record counter
        /// </summary>
        int recordCounter = 1;

        /// <summary>
        /// data watehouse
        /// </summary>
        readonly Dictionary<Guid, IDataWarehouse> repositoryWarehouses = new Dictionary<Guid, IDataWarehouse>();

        /// <summary>
        /// commit success event handler
        /// </summary>
        readonly List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> commitSuccessEventHandlers = new List<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>>();

        /// <summary>
        /// domain event collection
        /// </summary>
        readonly List<IDomainEvent> domainEventCollection = new List<IDomainEvent>();

        /// <summary>
        /// data access event collection
        /// </summary>
        readonly List<IDataAccessEvent> dataAccessEventCollection = new List<IDataAccessEvent>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the domain event manager
        /// </summary>
        public DomainEventManager DomainEventManager { get; private set; }

        /// <summary>
        /// Gets the domain events
        /// </summary>
        public IEnumerable<IDomainEvent> DomainEvents => domainEventCollection;

        /// <summary>
        /// Gets the data access events
        /// </summary>
        public IEnumerable<IDataAccessEvent> DataAccessEvents => dataAccessEventCollection;

        /// <summary>
        /// Gets the command count
        /// </summary>
        public int CommandCount
        {
            get
            {
                return commandCollection?.Count ?? 0;
            }
        }

        /// <summary>
        /// Gets the work id
        /// </summary>
        public string WorkId { get; } = string.Empty;

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region Methods

        #region Activation record

        #region Register activation record

        /// <summary>
        /// Add work activation
        /// </summary>
        /// <param name="records">New activation records</param>
        public void RegisterActivationRecord(params IActivationRecord[] records)
        {
            IEnumerable<IActivationRecord> recordValues = records;
            RegisterActivationRecord(recordValues);
        }

        /// <summary>
        /// Add activation operation
        /// </summary>
        /// <param name="records">New activation records</param>
        public void RegisterActivationRecord(IEnumerable<IActivationRecord> records)
        {
            if (records.IsNullOrEmpty())
            {
                return;
            }
            activationRecordCollection.AddRange(records);
        }

        #endregion

        #region Resolve activation record

        /// <summary>
        /// Resolve activation record
        /// </summary>
        void ResolveActivationRecord()
        {
            foreach (var record in activationRecordCollection)
            {
                if (record == null)
                {
                    continue;
                }
                ResolveSingleActivationRecord(record);
            }
        }

        /// <summary>
        /// Resolve single activation record
        /// </summary>
        /// <param name="record">Activation record</param>
        void ResolveSingleActivationRecord(IActivationRecord record)
        {
            if (record == null)
            {
                return;
            }
            IEnumerable<IActivationRecord> followRecords = null;
            switch (record.Operation)
            {
                case ActivationOperation.Package:
                    followRecords = record.GetFollowRecords();
                    break;
                default:
                    record.Id = GetActivationRecordId();
                    if (activationMaxRecordIds.TryGetValue(record.RecordIdentity, out int maxRecordId))
                    {
                        if (record.Id >= maxRecordId)
                        {
                            activationMaxRecordIds[record.RecordIdentity] = record.Id;
                        }
                    }
                    else
                    {
                        activationMaxRecordIds.Add(record.RecordIdentity, record.Id);
                    }
                    activationRecordValues[record.Id] = record;
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

        #region Get record id

        /// <summary>
        /// Get record id
        /// </summary>
        /// <returns>Return a new activation record id</returns>
        public int GetActivationRecordId()
        {
            return recordCounter++;
        }

        #endregion

        #endregion

        #region Command

        /// <summary>
        /// Build command
        /// </summary>
        void BuildCommand()
        {
            //resolve records
            ResolveActivationRecord();
            var recordIds = activationMaxRecordIds.Values.OrderBy(c => c);
            commandEngineGroups = new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>(activationMaxRecordIds.Count);
            commandCollection = new List<ICommand>(activationMaxRecordIds.Count);
            allowEmptyResultCommandCount = 0;
            foreach (var recordId in recordIds)
            {
                if (activationRecordValues.TryGetValue(recordId, out var record) && record != null)
                {
                    var command = record.GetExecuteCommand();
                    if (command?.IsObsolete ?? true)
                    {
                        if (allowTraceLog)
                        {
                            LogManager.LogInformation<DefaultWork>($"Execution command created based on the activity record {record.IdentityValue} are null or obsolete");
                        }
                        continue;
                    }

                    //trigger command executing event
                    var eventResult = command.TriggerStartingEvent();
                    if (eventResult?.BreakCommand ?? false)
                    {
                        if (allowTraceLog)
                        {
                            LogManager.LogInformation<DefaultWork>($"The execution command created by active record {record.IdentityValue} is blocked by the event handler：{eventResult?.Message}");
                        }
                    }

                    commandCollection.Add(command);
                    if (!command.MustReturnValueOnSuccess)
                    {
                        allowEmptyResultCommandCount += 1;
                    }
                    var commandEngines = CommandExecuteManager.GetCommandExecutors(command);
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
                        if (!commandEngineGroups.TryGetValue(engineKey, out Tuple<ICommandExecutor, List<ICommand>> engineValues))
                        {
                            engineValues = new Tuple<ICommandExecutor, List<ICommand>>(engine, new List<ICommand>(activationRecordValues.Count));
                        }
                        engineValues.Item2.Add(command);
                        commandEngineGroups[engineKey] = engineValues;
                    }
                }
            }
        }

        /// <summary>
        /// Get command id
        /// </summary>
        /// <returns>Return a new command id</returns>
        public int GetCommandId()
        {
            return commandCounter++;
        }

        /// <summary>
        /// Trigger command  callback event
        /// </summary>
        /// <param name="success">Whether command was executed successful</param>
        void TriggerCommandCallbackEvent(bool success)
        {
            if (commandCollection.IsNullOrEmpty())
            {
                return;
            }
            ThreadPool.QueueUserWorkItem(s =>
            {
                foreach (var command in commandCollection)
                {
                    var callbackCommand = command;
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        callbackCommand.TriggerCallbackEvent(success);
                    });
                }
            });
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        public WorkCommitResult Commit()
        {
            return CommitAsync().Result;
        }

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        public async Task<WorkCommitResult> CommitAsync()
        {
            try
            {
                if (allowTraceLog)
                {
                    LogManager.LogInformation<DefaultWork>($"===== Work：{WorkId} commit begin =====");
                }

                //build commands
                BuildCommand();
                WorkCommitResult commitResult = null;
                if (commandEngineGroups.IsNullOrEmpty())
                {
                    commitResult = WorkCommitResult.Empty();
                }
                else
                {
                    var executeOptions = GetCommandExecuteOptions();

                    if (allowTraceLog)
                    {
                        LogManager.LogInformation<DefaultWork>($"Work：{WorkId},Command execute options：{JsonSerializeHelper.ObjectToJson(executeOptions)}");
                        LogManager.LogInformation<DefaultWork>($"Work：{WorkId},Command engine keys：{string.Join(",", commandEngineGroups.Keys)},Command count：{commandCollection.Count}");
                    }

                    int returnValue = await CommandExecuteManager.ExecuteAsync(executeOptions, commandEngineGroups.Values).ConfigureAwait(false);
                    commitResult = new WorkCommitResult()
                    {
                        CommitCommandCount = commandCollection.Count,
                        ExecutedDataCount = returnValue,
                        AllowEmptyResultCommandCount = allowEmptyResultCommandCount
                    };
                }

                // trigger command callback event
                TriggerCommandCallbackEvent(commitResult.EmptyResultOrSuccess);
                if (commitResult.EmptyResultOrSuccess)
                {
                    //Trigger commit success event
                    TriggerCommitSuccessEvent(commitResult);
                    //Trigger work global success event
                    WorkManager.TriggerWorkCommitSuccessEvent(this, commitResult, commandCollection);
                    //Execute domain event
                    TriggerWorkCompletedDomainEvent();
                    //Execute data access event
                    TriggerDataAccessEvent();
                }
                else
                {
                    //Trigger work global commit fail event
                    WorkManager.TriggerWorkCommitFailEvent(this, commitResult, commandCollection);
                }

                if (allowTraceLog)
                {
                    LogManager.LogInformation<DefaultWork>($"Work：{WorkId},Commit command count：{commitResult.CommitCommandCount},Execute data count：{commitResult.ExecutedDataCount},Allow empty result command result：{allowEmptyResultCommandCount}");
                }

                return commitResult;
            }
            catch (Exception ex)
            {
                Reset();
                if (allowTraceLog)
                {
                    LogManager.LogInformation<DefaultWork>($"Work：{WorkId},Exception message ： {ex.Message}");
                }
                throw ex;
            }
            finally
            {
                if (allowTraceLog)
                {
                    LogManager.LogInformation<DefaultWork>($"===== Work：{WorkId} commit end =====");
                }
            }
        }

        /// <summary>
        /// Get command execute options
        /// </summary>
        /// <returns>Return the command execute options</returns>
        CommandExecuteOptions GetCommandExecuteOptions()
        {
            if (!IsolationLevel.HasValue)
            {
                return CommandExecuteOptions.Default;
            }
            return new CommandExecuteOptions()
            {
                IsolationLevel = IsolationLevel
            };
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        public void Rollback()
        {
            // unit work global rollback event handler
            WorkManager.TriggerWorkRollbackEvent(this);
            Reset();
        }

        /// <summary>
        /// Reset work
        /// </summary>
        void Reset()
        {
            commandCounter = 0;
            recordCounter = 0;
            allowEmptyResultCommandCount = 0;
            DomainEventManager.Reset();
            commandCollection?.Clear();
            commandEngineGroups?.Clear();
            activationRecordCollection?.Clear();
            repositoryWarehouses?.Clear();
            domainEventCollection?.Clear();
            dataAccessEventCollection?.Clear();
            activationMaxRecordIds.Clear();
            activationRecordValues.Clear();
            commitSuccessEventHandlers.Clear();
        }

        #endregion

        #region Warehouse

        /// <summary>
        /// Get data warehouse by entity type
        /// </summary>
        /// <returns>Return entity data warehouse</returns>
        public DataWarehouse<TEntity> GetWarehouse<TEntity>() where TEntity : BaseEntity<TEntity>, new()
        {
            var entityTypeId = typeof(TEntity).GUID;
            if (!repositoryWarehouses.TryGetValue(entityTypeId, out var warehouse))
            {
                warehouse = new DataWarehouse<TEntity>();
                repositoryWarehouses.Add(entityTypeId, warehouse);
            }
            return warehouse as DataWarehouse<TEntity>;
        }

        #endregion

        #region Work event

        #region Subscribe commit success event

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public void SubscribeCommitSuccessEvent(params Action<IWork, WorkCommitResult, IEnumerable<ICommand>>[] eventHandlers)
        {
            IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> handlerCollection = eventHandlers;
            SubscribeCommitSuccessEvent(handlerCollection);
        }

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        public void SubscribeCommitSuccessEvent(IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> eventHandlers)
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            commitSuccessEventHandlers.AddRange(eventHandlers);
        }

        #endregion

        #region Trigger commit success event handler

        /// <summary>
        /// Trigger commit success event
        /// </summary>
        /// <param name="commitResult">Work commit result</param>
        void TriggerCommitSuccessEvent(WorkCommitResult commitResult)
        {
            foreach (var handler in commitSuccessEventHandlers)
            {
                var eventHandler = handler;
                ThreadPool.QueueUserWorkItem(s => { eventHandler(this, commitResult, commandCollection); });
            }
        }

        #endregion

        #endregion

        #region Domain event

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        public void PublishDomainEvent(params IDomainEvent[] domainEvents)
        {
            IEnumerable<IDomainEvent> eventCollection = domainEvents;
            PublishDomainEvent(eventCollection);
        }

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        public void PublishDomainEvent(IEnumerable<IDomainEvent> domainEvents)
        {
            if (domainEvents.IsNullOrEmpty())
            {
                return;
            }
            domainEventCollection.AddRange(domainEvents);
            DomainEventManager.Publish(domainEvents);
        }

        /// <summary>
        /// Trigger work completed domain event
        /// </summary>
        void TriggerWorkCompletedDomainEvent()
        {
            //Local event handler
            DomainEventManager?.TriggerDomainEvent(EventTriggerTime.WorkCompleted, DomainEvents);

            //Global event handler
            DomainEventBus.GlobalDomainEventManager?.TriggerDomainEvent(EventTriggerTime.WorkCompleted, DomainEvents);
        }

        #endregion

        #region Data access event

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataEvents">Data access events</param>
        public void PublishDataAccessEvent(IEnumerable<IDataAccessEvent> dataEvents)
        {
            if (dataEvents.IsNullOrEmpty())
            {
                return;
            }
            dataAccessEventCollection.AddRange(dataEvents);
        }

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataEvents">Data access event</param>
        public void PublishDataAccessEvent(params IDataAccessEvent[] dataEvents)
        {
            IEnumerable<IDataAccessEvent> eventCollection = dataEvents;
            PublishDataAccessEvent(eventCollection);
        }

        /// <summary>
        /// Trigger data access event
        /// </summary>
        /// <returns></returns>
        void TriggerDataAccessEvent()
        {
            DataAccessEventBus.TriggerDataAccessEvent(dataAccessEventCollection);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            WorkManager.Current = null;
        }

        #endregion

        #endregion
    }
}
