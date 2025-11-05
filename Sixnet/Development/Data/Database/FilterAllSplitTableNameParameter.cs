// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    public class FilterAllSplitTableNameParameter
    {
        public List<string> AllTableNames { get; set; }

        public string RootTableName { get; set; }

        public SplitTableBehavior Behavior { get; set; }
    }
}
