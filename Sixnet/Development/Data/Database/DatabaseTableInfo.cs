using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database table info
    /// </summary>
    public class DatabaseTableInfo
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DatabaseTableInfo targetTable)
            {
                return string.Equals(TableName, targetTable.TableName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return TableName?.GetHashCode() ?? 0;
        }
    }

    public class DatabaseTableNameComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj?.ToUpper().GetHashCode() ?? 0;
        }
    }
}
