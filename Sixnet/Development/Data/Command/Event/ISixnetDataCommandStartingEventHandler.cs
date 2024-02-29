using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Defines data command starting event handler contract
    /// </summary>
    public interface ISixnetDataCommandStartingEventHandler
    {
        /// <summary>
        /// Handle data command event
        /// </summary>
        /// <param name="dataCommandStartingEvent">Data command starting event</param>
        /// <returns>Execution reslt</returns>
        void Handle(SixnetDataCommandStartingEvent dataCommandStartingEvent);
    }
}
