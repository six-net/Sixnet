// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database single command
    /// </summary>
    public class SixnetSingleDatabaseCommand : SixnetDatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data command
        /// </summary>
        public SixnetDataCommand DataCommand { get; set; }
    }
}
