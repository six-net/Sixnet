using System.Data;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database execution statement
    /// </summary>
    public abstract class DatabaseStatement
    {
        /// <summary>
        /// Gets or sets the statement script
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// Gets or sets the command parameters
        /// </summary>
        public DataCommandParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the script type
        /// </summary>
        public CommandType ScriptType { get; set; } = CommandType.Text;
    }
}
