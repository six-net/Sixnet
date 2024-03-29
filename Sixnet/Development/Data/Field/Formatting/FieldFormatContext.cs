﻿using Sixnet.Development.Data.Database;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field format context
    /// </summary>
    public class FieldFormatContext
    {
        /// <summary>
        /// Gets or sets the field format option
        /// </summary>
        public FieldFormatOption FormatOption { get; set; }

        /// <summary>
        /// Gets or sets the database server
        /// </summary>
        public DatabaseServer Server { get; set; }

        /// <summary>
        /// Gets or sets the table pet name
        /// </summary>
        public string TablePetName { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the field location
        /// </summary>
        public FieldLocation FieldLocation { get; set; } = FieldLocation.Output;

        /// <summary>
        /// Gets or sets the query location
        /// </summary>
        public QueryableLocation QueryLocation { get; set; } = QueryableLocation.Top;
    }
}
