using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database multiple command
    /// </summary>
    public class MultipleDatabaseCommand : SixnetDatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data operation commands
        /// </summary>
        public List<SixnetDataCommand> DataCommands { get; set; }
    }
}
