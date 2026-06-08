// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    public class SixnetGetSplitTableNameParameter
    {
        /// <summary>
        /// Split behavior
        /// </summary>
        public SixnetSplitTableBehavior Behavior { get; set; }

        /// <summary>
        /// Root table name
        /// </summary>
        public SixnetDatabaseObjectName RootTableName { get; set; }

        /// <summary>
        /// All table names
        /// </summary>
        public List<SixnetDatabaseObjectName> AllTableNames { get; set; }

        /// <summary>
        /// Resolved table names
        /// </summary>
        public List<SixnetDatabaseObjectName> ResolvedTableNames { get; set; }
    }
}
