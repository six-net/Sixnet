// "Company © 2025. All rights reserved."

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    public struct DatabaseObjectName: IComparable<DatabaseObjectName>
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema name
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Database object name type
        /// </summary>
        public DatabaseObjectType Type { get; set; }

        /// <summary>
        /// Gets the object full name
        /// </summary>
        internal string FullName => string.IsNullOrWhiteSpace(SchemaName) ? Name : $"{SchemaName}.{Name}";

        /// <summary>
        /// Gets the object identity name
        /// </summary>
        public string IdentityName => string.IsNullOrWhiteSpace(SchemaName) ? Name : $"{SchemaName}_{Name}";

        public static DatabaseObjectName Create(string name, DatabaseObjectType nameType, string schemaName = "")
        {
            return new DatabaseObjectName()
            {
                Name = name,
                SchemaName = schemaName,
                Type = nameType,
            };
        }

        public DatabaseObjectName Clone()
        {
            return new DatabaseObjectName()
            {
                Name = Name,
                SchemaName = SchemaName,
                Type = Type
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is DatabaseObjectName targetObjectName)
            {
                return targetObjectName.Type == Type
                    && string.Equals(targetObjectName.SchemaName ?? string.Empty, SchemaName ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(targetObjectName.Name ?? string.Empty, Name ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return $"{Type}.{SchemaName}.{Name}".GetHashCode();
        }

        public int CompareTo(DatabaseObjectName other)
        {
            return string.Compare(IdentityName, other.IdentityName);
        }
    }
}
