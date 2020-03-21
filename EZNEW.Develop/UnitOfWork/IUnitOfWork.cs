using EZNEW.Develop.Command;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Event;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// IUnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region propertys

        /// <summary>
        /// command count
        /// </summary>
        int CommandCount { get; }

        /// <summary>
        /// work id
        /// </summary>
        string WorkId { get; }

        /// <summary>
        /// domain event manager
        /// </summary>
        DomainEventManager DomainEventManager { get; }

        /// <summary>
        /// domain events
        /// </summary>
        List<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region methods

        #region commit

        /// <summary>
        /// Commit Command
        /// </summary>
        /// <returns></returns>
        CommitResult Commit();

        /// <summary>
        /// Commit Command
        /// </summary>
        /// <returns></returns>
        Task<CommitResult> CommitAsync();

        #endregion

        #region activation record

        /// <summary>
        /// add activation operation
        /// </summary>
        /// <param name="records"></param>
        void AddActivation(params IActivationRecord[] records);

        /// <summary>
        /// get new record id
        /// </summary>
        /// <returns></returns>
        int GetRecordId();

        #endregion

        #region command

        /// <summary>
        /// get command id
        /// </summary>
        /// <returns></returns>
        int GetCommandId();

        #endregion

        #region warehouse

        /// <summary>
        /// get data warehouse by entity type
        /// </summary>
        /// <returns></returns>
        DataWarehouse<ET> GetWarehouse<ET>() where ET : BaseEntity<ET>,new();

        #endregion

        #region domain event

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        void PublishDomainEvent(params IDomainEvent[] domainEvents);

        /// <summary>
        /// publish domain event
        /// </summary>
        /// <param name="domainEvents">domain event</param>
        Task PublishDomainEventAsync(params IDomainEvent[] domainEvents);

        #endregion

        #region work event

        #region register commit success event handler

        /// <summary>
        /// register commit success event handler
        /// </summary>
        /// <param name="handlers">handlers</param>
        void RegisterCommitSuccessEventHandler(params Action<IUnitOfWork, CommitResult, IEnumerable<ICommand>>[] handlers);

        #endregion

        #endregion

        #endregion
    }
}
