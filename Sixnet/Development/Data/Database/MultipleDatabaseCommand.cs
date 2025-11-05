// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database multiple command
    /// </summary>
    public class MultipleDatabaseCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data operation commands
        /// </summary>
        public List<SixnetDataCommand> DataCommands { get; set; }
    }
}
