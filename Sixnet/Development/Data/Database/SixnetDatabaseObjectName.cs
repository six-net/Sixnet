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
    public struct SixnetDatabaseObjectName: IComparable<SixnetDatabaseObjectName>
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
        public SixnetDatabaseObjectType Type { get; set; }

        /// <summary>
        /// Gets the object full name
        /// </summary>
        internal string FullName => string.IsNullOrWhiteSpace(SchemaName) ? Name : $"{SchemaName}.{Name}";

        /// <summary>
        /// Gets the object identity name
        /// </summary>
        public string IdentityName => string.IsNullOrWhiteSpace(SchemaName) ? Name : $"{SchemaName}_{Name}";

        public override string ToString()
        {
            return FullName;
        }

        public static SixnetDatabaseObjectName Create(string name, SixnetDatabaseObjectType nameType, string schemaName = "")
        {
            return new SixnetDatabaseObjectName()
            {
                Name = name,
                SchemaName = schemaName,
                Type = nameType,
            };
        }

        public SixnetDatabaseObjectName Clone()
        {
            return new SixnetDatabaseObjectName()
            {
                Name = Name,
                SchemaName = SchemaName,
                Type = Type
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is SixnetDatabaseObjectName targetObjectName)
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

        public int CompareTo(SixnetDatabaseObjectName other)
        {
            return string.Compare(IdentityName, other.IdentityName);
        }
    }
}
