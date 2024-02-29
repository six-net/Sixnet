using System;
using System.Collections.Generic;
using System.Linq;
using Sixnet.Model;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Split table behavior
    /// </summary>
    public class SplitTableBehavior : ISixnetCloneableModel<SplitTableBehavior>
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
        /// </summary>
        public Func<IEnumerable<string>, IEnumerable<string>> SplitTableNameFilter { get; set; }

        public SplitTableBehavior Clone()
        {
            return new SplitTableBehavior()
            {
                SplitValues = SplitValues?.Select(v => v).ToList(),
                SplitTableNameFilter = SplitTableNameFilter,
                SelectionPattern = SelectionPattern
            };
        }

        public bool IsTakeAllSplitTables(IEnumerable<string> splitTableNames)
        {
            return splitTableNames.IsNullOrEmpty() && SplitValues.IsNullOrEmpty() && SplitTableNameFilter == null;
        }
    }
}
