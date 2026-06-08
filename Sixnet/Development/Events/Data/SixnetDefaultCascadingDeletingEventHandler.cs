// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Cascading delete event handler
    /// </summary>
    internal class SixnetDefaultCascadingDeletingEventHandler<TRelationEntity> : ISixnetEventHandler
    {
        public SixnetEventHandlerOptions Options { get; set; }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="dataEvent"></param>
        /// <returns></returns>
        public Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(eventData is not SixnetCascadingDeletingDataEvent, "Event is not a deleting event");

            var deletingDataEvent = eventData as SixnetCascadingDeletingDataEvent;
            var dataClient = deletingDataEvent.DataClient;
            var deleteRelationEntityQueryable = SixnetQuerier.Create<TRelationEntity>().Join(new SixnetJoinEntry()
            {
                Type = SixnetJoinType.InnerJoin,
                Target = deletingDataEvent.Command?.Queryable
            });
            dataClient.Delete(deleteRelationEntityQueryable, deletingDataEvent.Command.Options);
            return Task.CompletedTask;
        }
    }
}
