using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Cache.String.Parameters;

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
        public string RootTableName { get; set; }

        /// <summary>
        /// All table names
        /// </summary>
        public List<string> AllTableNames { get; set; }

        /// <summary>
        /// Resolved table names
        /// </summary>
        public List<string> ResolvedTableNames { get; set; }
    }
}
