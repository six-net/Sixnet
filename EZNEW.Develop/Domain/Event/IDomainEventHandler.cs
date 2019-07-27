using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// domain event handler
    /// </summary>
    public interface IDomainEventHandler
    {
        #region Propertys

        /// <summary>
        /// execute time
        /// </summary>
        EventExecuteTime ExecuteTime { get; }

        #endregion

        #region execute event

        /// <summary>
        /// execute domain event
        /// </summary>
        /// <param name="domainEvent">domain event</param>
        /// <returns></returns>
        DomainEventExecuteResult Execute(IDomainEvent domainEvent);

        /// <summary>
        /// execute domain event
        /// </summary>
        /// <param name="domainEvent">domain event</param>
        /// <returns></returns>
        Task<DomainEventExecuteResult> ExecuteAsync(IDomainEvent domainEvent);

        #endregion
    }
}
