// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

namespace Sixnet.Development.Data.Command.Events
{
    /// <summary>
    /// Defines data command callback event handler contract
    /// </summary>
    public interface ISixnetDataCommandCallbackEventHandler
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="dataCommandCallbackEvent">Data command callback event</param>
        Task ExecuteAsync(SixnetDataCommandCallbackEvent dataCommandCallbackEvent);
    }
}
