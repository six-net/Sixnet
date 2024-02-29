using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Defaines data command starting event
    /// </summary>
    [Serializable]
    public class SixnetDataCommandStartingEvent
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetDataCommand Command { get; set; }
    }
}
