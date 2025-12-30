// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database table info
    /// </summary>
    public class SixnetDataTable
    {
        /// <summary>
        /// Gets or sets the id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the schema id
        /// </summary>
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema name
        /// </summary>
        public string SchemaName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SixnetDataTable targetTable)
            {
                return string.Equals(SchemaName, targetTable.SchemaName, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(Name, targetTable.Name, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return $"{SchemaName}_{Name}".GetHashCode();
        }

        public DatabaseObjectName GetDatabaseObjectName()
        {
            return DatabaseObjectName.Create(Name, DatabaseObjectType.Table, SchemaName);
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
