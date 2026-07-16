// "Company © 2025. All rights reserved."

using Sixnet.Model;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Split table behavior
    /// </summary>
    public class SixnetSplitTableBehavior : ISixnetCloneable<SixnetSplitTableBehavior>
    {
        /// <summary>
        /// Gets or sets the split values
        /// </summary>
        public IEnumerable<dynamic> SplitValues { get; set; }

        /// <summary>
        /// Gets or sets the specific table names
        /// </summary>
        public IEnumerable<SixnetDatabaseObjectName> SpecificTableNames { get; set; }

        /// <summary>
        /// Gets or sets the table name selection pattern
        /// </summary>
        public SixnetSplitTableNameSelectionPattern SelectionPattern { get; set; }

        /// <summary>
        /// Gets or sets the split table name filter
        /// Arg1: All table names,
        /// Arg2: Resolved table names
        /// </summary>
        public Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> SplitTableNameFilter { get; set; }

        public SixnetSplitTableBehavior Clone()
        {
            return new SixnetSplitTableBehavior()
            {
                SplitValues = SplitValues?.Select(v => v).ToList(),
                SplitTableNameFilter = SplitTableNameFilter,
                SelectionPattern = SelectionPattern,
                SpecificTableNames = SpecificTableNames?.Select(c => c).ToList()
            };
        }

        public bool IsTakeAllSplitTables(IEnumerable<SixnetDatabaseObjectName> splitTableNames)
        {
            return splitTableNames.IsNullOrEmpty() && SplitValues.IsNullOrEmpty() && SpecificTableNames.IsNullOrEmpty() && SplitTableNameFilter == null;
        }
    }
}
