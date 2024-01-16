using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Defines data command callback event handler contract
    /// </summary>
    public interface IDataCommandCallbackEventHandler
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="dataCommandCallbackEvent">Data command callback event</param>
        Task ExecuteAsync(DataCommandCallbackEvent dataCommandCallbackEvent);
    }
}
