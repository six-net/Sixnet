using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database single command
    /// </summary>
    public class DatabaseSingleCommand : DatabaseCommand
    {
        /// <summary>
        /// Gets or sets the data command
        /// </summary>
        public DataCommand DataCommand { get; set; }
    }
}
