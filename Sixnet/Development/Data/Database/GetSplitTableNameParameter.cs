// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    public class GetSplitTableNameParameter
    {
        /// <summary>
        /// Split behavior
        /// </summary>
        public SplitTableBehavior Behavior { get; set; }

        /// <summary>
        /// Root table name
        /// </summary>
        public DatabaseObjectName RootTableName { get; set; }

        /// <summary>
        /// All table names
        /// </summary>
        public List<DatabaseObjectName> AllTableNames { get; set; }

        /// <summary>
        /// Resolved table names
        /// </summary>
        public List<DatabaseObjectName> ResolvedTableNames { get; set; }
    }
}
