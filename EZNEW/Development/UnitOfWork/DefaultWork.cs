using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.Development.Command;
using EZNEW.Development.DataAccess;
using EZNEW.Development.DataAccess.Event;
using EZNEW.Development.Domain.Event;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Domain.Repository.Warehouse.Storage;
using EZNEW.Development.Entity;
using EZNEW.Diagnostics;
using EZNEW.Logging;
using EZNEW.Serialization;

namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Default implements for work
    /// </summary>
    internal class DefaultWork : IWork
    {
        /// <summary>
        /// Initialize default work
        /// </summary>
        internal DefaultWork()
        {
            WorkId = Guid.NewGuid().ToString();
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
        /// Command executor
        /// Key:Executor key
        /// </summary>
        Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>> commandExecutorGroups = null;

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
        /// entity storage
        /// </summary>
        readonly Dictionary<Guid, object> entityStorageCollection = new Dictionary<Guid, object>();

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
            commandExecutorGroups = new Dictionary<string, Tuple<ICommandExecutor, List<ICommand>>>(activationMaxRecordIds.Count);
            commandCollection = new List<ICommand>(activationMaxRecordIds.Count);
            allowEmptyResultCommandCount = 0;
            foreach (var recordId in recordIds)
            {
                if (activationRecordValues.TryGetValue(recordId, out var record) && record != null)
                {
                    var command = record.GetExecutionCommand();
                    if (command?.IsObsolete ?? true)
                    {
                        FrameworkLogManager.ObsoleteActivationRecord(this, record, command);
                        continue;
                    }

                    //trigger command executing event
                    var eventResult = command.TriggerStartingEvent();
                    if (eventResult?.BreakCommand ?? false)
                    {
                        FrameworkLogManager.BreakActivationRecord(this, record, command);
                        continue;
                    }

                    commandCollection.Add(command);
                    if (!command.MustAffectedData)
                    {
                        allowEmptyResultCommandCount += 1;
                    }
                    var commandExecutors = CommandExecutionManager.GetCommandExecutors(command);
                    if (commandExecutors.IsNullOrEmpty())
                    {
                        continue;
                    }
                    foreach (var executor in commandExecutors)
                    {
                        if (executor == null)
                        {
                            continue;
                        }
                        var executorKey = executor.IdentityValue;
                        if (!commandExecutorGroups.TryGetValue(executorKey, out Tuple<ICommandExecutor, List<ICommand>> executorValues))
                        {
                            executorValues = new Tuple<ICommandExecutor, List<ICommand>>(executor, new List<ICommand>(activationRecordValues.Count));
                        }
                        executorValues.Item2.Add(command);
                        commandExecutorGroups[executorKey] = executorValues;
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
            var workCommands = commandCollection.Select(c => c).ToList();
            ThreadPool.QueueUserWorkItem(s =>
            {
                foreach (var command in workCommands)
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
                FrameworkLogManager.LogWorkStartSubmitting(this);
                //build commands
                BuildCommand();
                WorkCommitResult commitResult = null;
                if (commandExecutorGroups.IsNullOrEmpty())
                {
                    commitResult = WorkCommitResult.Empty();
                }
                else
                {
                    var executionOptions = GetCommandExecutionOptions();
                    FrameworkLogManager.LogWorkOptions(this, executionOptions);

                    int returnValue = await CommandExecutionManager.ExecuteAsync(executionOptions, commandExecutorGroups.Values).ConfigureAwait(false);
                    commitResult = new WorkCommitResult()
                    {
                        CommittedCommandCount = commandCollection.Count,
                        AffectedDataCount = returnValue,
                        AllowEmptyCommandCount = allowEmptyResultCommandCount
                    };
                }

                // trigger command callback event
                TriggerCommandCallbackEvent(commitResult.EmptyOrSuccess);
                if (commitResult.EmptyOrSuccess)
                {
                    //Trigger commit success event
                    TriggerCommitSuccessEvent(commitResult);
                    //Trigger work global success event
                    WorkManager.TriggerWorkCommitSuccessEvent(this, commitResult, commandCollection);
                    //Execute domain event
                    TriggerWorkCompletedDomainEvent();
                    //Execute data access event
                    TriggerDataAccessEvent();

                    FrameworkLogManager.LogWorkSubmittedSuccessfully(this, commitResult);
                }
                else
                {
                    //Trigger work global commit fail event
                    WorkManager.TriggerWorkCommitFailEvent(this, commitResult, commandCollection);

                    FrameworkLogManager.LogWorkSubmittedFailure(this, commitResult);
                }

                return commitResult;
            }
            catch (Exception ex)
            {
                FrameworkLogManager.LogWorkSubmittedException(this, ex);
                throw ex;
            }
            finally
            {
                Reset();
            }
        }

        /// <summary>
        /// Get command execution options
        /// </summary>
        /// <returns>Return the command execution options</returns>
        CommandExecutionOptions GetCommandExecutionOptions()
        {
            if (!IsolationLevel.HasValue)
            {
                return CommandExecutionOptions.Default;
            }
            return new CommandExecutionOptions()
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
            FrameworkLogManager.LogWorkRollback(this);
            // unit work global rollback event handler
            WorkManager.TriggerWorkRollbackEvent(this);
            Reset();
        }

        /// <summary>
        /// Reset work
        /// </summary>
        void Reset()
        {
            FrameworkLogManager.LogWorkReset(this);
            commandCounter = 0;
            recordCounter = 0;
            allowEmptyResultCommandCount = 0;
            DomainEventManager.Reset();
            commandCollection?.Clear();
            commandExecutorGroups?.Clear();
            activationRecordCollection?.Clear();
            entityStorageCollection?.Clear();
            domainEventCollection?.Clear();
            dataAccessEventCollection?.Clear();
            activationMaxRecordIds.Clear();
            activationRecordValues.Clear();
            commitSuccessEventHandlers.Clear();
        }

        #endregion

        #region Entity storage

        /// <summary>
        /// Get entity storage
        /// </summary>
        /// <returns>Entity storage</returns>
        public EntityStorage<TEntity> GetEntityStorage<TEntity>() where TEntity : BaseEntity<TEntity>, new()
        {
            var entityTypeId = typeof(TEntity).GUID;
            if (!entityStorageCollection.TryGetValue(entityTypeId, out var storage))
            {
                storage = new EntityStorage<TEntity>();
                entityStorageCollection.Add(entityTypeId, storage);
            }
            return storage as EntityStorage<TEntity>;
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
            DomainEventBus.DomainEventManager?.TriggerDomainEvent(EventTriggerTime.WorkCompleted, DomainEvents);
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
            FrameworkLogManager.LogWorkDispose(this);
            Reset();
            WorkManager.Current = null;

        }

        #endregion

        #endregion
    }
}
