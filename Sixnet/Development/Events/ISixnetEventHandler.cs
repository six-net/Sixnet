using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Event;
using System.Threading.Tasks;
using System.Threading;

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
        /// <returns>Return data access event result</returns>
        Task Handle(ISixnetEvent eventData, CancellationToken cancellationToken);
    }
}
