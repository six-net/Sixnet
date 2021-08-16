using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.Command;
using EZNEW.Development.DataAccess;
using EZNEW.Development.DataAccess.Event;
using EZNEW.Development.Domain.Event;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Entity;

namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Work contract
    /// </summary>
    public interface IWork : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the command count
        /// </summary>
        int CommandCount { get; }

        /// <summary>
        /// Gets or the work id
        /// </summary>
        string WorkId { get; }

        /// <summary>
        /// Gets the domain event manager
        /// </summary>
        DomainEventManager DomainEventManager { get; }

        /// <summary>
        /// Gets the domain events
        /// </summary>
        IEnumerable<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region Methods

        #region Commit

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        WorkCommitResult Commit();

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns>Return work commit result</returns>
        Task<WorkCommitResult> CommitAsync();

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        void Rollback();

        #endregion

        #region Activation record

        /// <summary>
        /// Register activation record
        /// </summary>
        /// <param name="records">Activation records</param>
        void RegisterActivationRecord(IEnumerable<IActivationRecord> records);

        /// <summary>
        /// Register activation record
        /// </summary>
        /// <param name="records">Activation records</param>
        void RegisterActivationRecord(params IActivationRecord[] records);

        /// <summary>
        /// Gets a new record id
        /// </summary>
        /// <returns>Return a new activation record id</returns>
        int GetActivationRecordId();

        #endregion

        #region Command

        /// <summary>
        /// Gets a command id
        /// </summary>
        /// <returns>Return a new command id</returns>
        int GetCommandId();

        #endregion

        #region Warehouse

        /// <summary>
        /// Get data warehouse by entity type
        /// </summary>
        /// <returns>Return entity data warehouse</returns>
        DataWarehouse<TEntity> GetWarehouse<TEntity>() where TEntity : BaseEntity<TEntity>, new();

        #endregion

        #region Domain event

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        void PublishDomainEvent(IEnumerable<IDomainEvent> domainEvents);

        /// <summary>
        /// Publish domain event
        /// </summary>
        /// <param name="domainEvents">Domain events</param>
        void PublishDomainEvent(params IDomainEvent[] domainEvents);

        #endregion

        #region Work event

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        void SubscribeCommitSuccessEvent(IEnumerable<Action<IWork, WorkCommitResult, IEnumerable<ICommand>>> eventHandlers);

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        void SubscribeCommitSuccessEvent(params Action<IWork, WorkCommitResult, IEnumerable<ICommand>>[] eventHandlers);

        #endregion

        #region Data access event

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataEvents">Data access events</param>
        void PublishDataAccessEvent(IEnumerable<IDataAccessEvent> dataEvents);

        /// <summary>
        /// Publish data access event
        /// </summary>
        /// <param name="dataEvents">Data access events</param>
        void PublishDataAccessEvent(params IDataAccessEvent[] dataEvents);

        #endregion

        #endregion
    }
}
