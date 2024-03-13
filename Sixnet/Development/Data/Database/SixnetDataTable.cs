using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database table info
    /// </summary>
    public class SixnetDataTable
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SixnetDataTable targetTable)
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

    public class SixnetDataTableNameComparer : IEqualityComparer<string>
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
