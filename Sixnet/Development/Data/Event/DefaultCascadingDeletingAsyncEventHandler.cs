using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Event;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using System.Threading.Tasks;
using System.Threading;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Cascading delete event handler
    /// </summary>
    internal class DefaultCascadingDeletingAsyncEventHandler<TRelationEntity> : ISixnetDataEventHandler
    {
        public EventHandlerOptions Options { get; set; }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="dataEvent"></param>
        /// <returns></returns>
        public Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(eventData is not CascadingDeletingAsyncDataEvent, "Event is not a deleting event");

            var deletingDataEvent = eventData as CascadingDeletingAsyncDataEvent;
            var dataClient = deletingDataEvent.DataClient;
            var deleteRelationEntityQueryable = SixnetQueryable.Create<TRelationEntity>().Join(new JoinEntry()
            {
                Type = JoinType.InnerJoin,
                TargetQueryable = deletingDataEvent.Command?.Queryable
            });
            return dataClient.DeleteAsync(deleteRelationEntityQueryable, deletingDataEvent.Command.Options);
        }
    }
}
