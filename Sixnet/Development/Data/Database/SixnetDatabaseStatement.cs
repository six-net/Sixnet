// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database execution statement
    /// </summary>
    public abstract class SixnetDatabaseStatement
    {
        /// <summary>
        /// Gets or sets the statement script
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the command parameters
        /// </summary>
        public SixnetDataCommandParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the script type
        /// </summary>
        public CommandType ScriptType { get; set; } = CommandType.Text;
    }
}
