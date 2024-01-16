using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database multiple command
    /// </summary>
    public class DatabaseMultipleCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data operation commands
        /// </summary>
        public List<DataCommand> DataCommands { get; set; }
    }
}
