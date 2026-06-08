// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Defines sixnet event handler
    /// </summary>
    public interface ISixnetEventHandler
    {
        /// <summary>
        /// Gets or sets the event handler options
        /// </summary>
        SixnetEventHandlerOptions Options { get; set; }

        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken);
    }
}
