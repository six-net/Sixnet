using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Events;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Cascading delete event handler
    /// </summary>
    internal class CascadingDeletingEventHandler<TRelationEntity> : IDataEventHandler
    {
        public SixnetEventHandlerOptions Options { get; set; }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="dataEvent"></param>
        /// <returns></returns>
        public Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken)
        {
            ThrowHelper.ThrowFrameworkErrorIf(eventData is not DeletingDataEvent, "Event is not a deleting event");

            var deletingDataEvent = eventData as DeletingDataEvent;
            var dataClient = deletingDataEvent.DataClient;
            var deleteRelationEntityQueryable = SixnetQueryable.Create<TRelationEntity>().Join(new JoinEntry()
            {
                Type = JoinType.InnerJoin,
                TargetQueryable = deletingDataEvent.Command?.Queryable
            });
            dataClient.Delete(deleteRelationEntityQueryable, deletingDataEvent.Command.Options);
            return Task.CompletedTask;
        }
    }
}
