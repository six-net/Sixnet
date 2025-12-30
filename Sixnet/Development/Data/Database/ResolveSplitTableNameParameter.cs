// "Company © 2025. All rights reserved."

using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Get split table name parameter
    /// </summary>
    public class ResolveSplitTableNameParameter
    {
        /// <summary>
        /// Gets or sets the  entity configuration
        /// </summary>
        public EntityConfiguration EntityConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the root table name
        /// </summary>
        public DatabaseObjectName RootTableName { get; set; }

        /// <summary>
        /// Gets or sets the split behavior
        /// </summary>
        public SplitTableBehavior SplitBehavior { get; set; }

        /// <summary>
        /// Gets or sets the expansion num
        /// </summary>
        public int ExpansionNum {  get; set; }
    }
}
