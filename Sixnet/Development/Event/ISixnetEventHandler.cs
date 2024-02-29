using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Event
{
    /// <summary>
    /// Defines sixnet event handler
    /// </summary>
    public interface ISixnetEventHandler
    {
        /// <summary>
        /// Gets or sets the event handler options
        /// </summary>
        EventHandlerOptions Options { get; set; }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken);
    }
}
