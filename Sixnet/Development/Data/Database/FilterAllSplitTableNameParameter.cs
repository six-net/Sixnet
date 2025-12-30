// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    public class FilterAllSplitTableNameParameter
    {
        public List<DatabaseObjectName> AllTableNames { get; set; }

        public DatabaseObjectName RootTableName { get; set; }

        public SplitTableBehavior Behavior { get; set; }
    }
}
