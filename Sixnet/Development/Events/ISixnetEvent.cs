using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Defines event contract
    /// </summary>
    public interface ISixnetEvent
    {
        /// <summary>
        /// Gets or sets the event id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        DateTimeOffset CreatedDate { get; set; }
    }
}
