// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Command.Events
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
