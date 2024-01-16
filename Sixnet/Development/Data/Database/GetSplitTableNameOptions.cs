using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Get split table name options
    /// </summary>
    public class GetSplitTableNameOptions
    {
        /// <summary>
        /// Gets or sets the  entity configuration
        /// </summary>
        public EntityConfiguration EntityConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the root table name
        /// </summary>
        public string RootTableName { get; set; }

        /// <summary>
        /// Gets or sets the split behavior
        /// </summary>
        public SplitTableBehavior SplitBehavior { get; set; }
    }
}
