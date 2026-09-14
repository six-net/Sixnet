// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    public class SixnetDatabaseScriptInfo
    {
        public List<string> Scripts { get; set; }
    }

    public class SixnetGetCreateTableDefineScriptParameter
    {
        public SixnetMigrationInfo MigrationInfo { get; set; }

        public SixnetColumnDefineInfo ColumnDefineInfo { get; set; }

        public SixnetDatabaseObjectName Table { get; set; }
    }

    public class SixnetDeleteAllTableParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema {  get; set; }
    }

    public class SixnetRenameTableParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the current table name
        /// </summary>
        public SixnetDatabaseObjectName CurrentTableName {  get; set; }

        /// <summary>
        /// Gets or sets the new table name
        /// </summary>
        public SixnetDatabaseObjectName NewTableName { get; set; }
    }

    public class SixnetAddFieldParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the table
        /// </summary>
        public SixnetDatabaseObjectName Table { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public List<SixnetDataField> Fields { get; set; }
    }

    public class SixnetUpdateFieldParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the table
        /// </summary>
        public SixnetDatabaseObjectName Table { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public Dictionary<string, SixnetDataField> Fields { get; set; }
    }

    public class SixnetDeleteFieldParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the table
        /// </summary>
        public SixnetDatabaseObjectName Table { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public List<SixnetDataField> Fields { get; set; }
    }

    /// <summary>
    /// Sixnet create column define parameter
    /// </summary>
    public class SixnetGetCreateColumnDefineParameter
    {
        public SixnetMigrationInfo MigrationInfo { get; set; }

        public SixnetEntityConfiguration EntityConfiguration { get; set; }
    }

    public class SixnetColumnDefineInfo
    {
        /// <summary>
        /// Gets or sets the column scripts
        /// </summary>
        public List<string> ColumnScripts { get; set; }

        /// <summary>
        /// Gets or sets primary keys
        /// </summary>
        public List<string> PrimaryKeys { get; set; }
    }


    public class SixnetAddForeignKeyParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the foreign key info
        /// </summary>
        public SixnetEntityForeignKeyInfo ForeignKeyInfo { get; set; }
    }

    public class SixnetDeleteForeignKeyParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the foreign key info
        /// </summary>
        public SixnetEntityForeignKeyInfo ForeignKeyInfo { get; set; }
    }

    public class SixnetDeleteAllForeignKeyParameter 
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema {  get; set; }
    }

    /// <summary>
    /// Add index parameter
    /// </summary>
    public class SixnetAddIndexParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the index info
        /// </summary>
        public SixnetEntityIndexInfo IndexInfo { get; set; }
    }

    /// <summary>
    /// Delete index parameter
    /// </summary>
    public class SixnetDeleteIndexParameter()
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the index info
        /// </summary>
        public SixnetEntityIndexInfo IndexInfo { get; set; }
    }

    /// <summary>
    /// Delete all view parameter
    /// </summary>
    public class SixnetDeleteAllViewParameter()
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema { get; set; }
    }

    /// <summary>
    /// Delete all function parameter
    /// </summary>
    public class SixnetDeleteAllFunctionParameter()
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema { get; set; }
    }

    /// <summary>
    /// Delete all customer type parameter
    /// </summary>
    public class SixnetDeleteAllCustomerTypeParameter()
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema { get; set; }
    }

    /// <summary>
    /// Delete all Procedure parameter
    /// </summary>
    public class SixnetDeleteAllProcedureParameter()
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetMigrationDatabaseCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the schema
        /// </summary>
        public string Schema { get; set; }
    }
}
