// "Company © 2025. All rights reserved."

using Sixnet.Model;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Split table behavior
    /// </summary>
    public class SplitTableBehavior : ISixnetCloneable<SplitTableBehavior>
    {
        /// <summary>
        /// Gets or sets the split values
        /// </summary>
        public IEnumerable<dynamic> SplitValues { get; set; }

        /// <summary>
        /// Gets or sets the table name selection pattern
        /// </summary>
        public SplitTableNameSelectionPattern SelectionPattern { get; set; }

        /// <summary>
        /// Gets or sets the split table name filter
        /// Arg1: All table names,
        /// Arg2: Resolved table names
        /// </summary>
        public Func<IEnumerable<DatabaseObjectName>, IEnumerable<DatabaseObjectName>, IEnumerable<DatabaseObjectName>> SplitTableNameFilter { get; set; }

        public SplitTableBehavior Clone()
        {
            return new SplitTableBehavior()
            {
                SplitValues = SplitValues?.Select(v => v).ToList(),
                SplitTableNameFilter = SplitTableNameFilter,
                SelectionPattern = SelectionPattern
            };
        }

        public bool IsTakeAllSplitTables(IEnumerable<DatabaseObjectName> splitTableNames)
        {
            return splitTableNames.IsNullOrEmpty() && SplitValues.IsNullOrEmpty() && SplitTableNameFilter == null;
        }
    }
}
