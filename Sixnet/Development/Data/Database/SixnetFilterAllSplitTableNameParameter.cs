// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    public class SixnetFilterAllSplitTableNameParameter
    {
        public List<SixnetDatabaseObjectName> AllTableNames { get; set; }

        public SixnetDatabaseObjectName RootTableName { get; set; }

        public SixnetSplitTableBehavior Behavior { get; set; }
    }
}
