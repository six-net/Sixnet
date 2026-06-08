// "Company © 2025. All rights reserved."

using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Get split table name parameter
    /// </summary>
    public class SixnetResolveSplitTableNameParameter
    {
        /// <summary>
        /// Gets or sets the  entity configuration
        /// </summary>
        public SixnetEntityConfiguration EntityConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the root table name
        /// </summary>
        public SixnetDatabaseObjectName RootTableName { get; set; }

        /// <summary>
        /// Gets or sets the split behavior
        /// </summary>
        public SixnetSplitTableBehavior SplitBehavior { get; set; }

        /// <summary>
        /// Gets or sets the expansion num
        /// </summary>
        public int ExpansionNum {  get; set; }
    }
}
