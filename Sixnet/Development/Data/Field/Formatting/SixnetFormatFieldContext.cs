// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field format context
    /// </summary>
    public class SixnetFormatFieldContext
    {
        /// <summary>
        /// Gets or sets the field format setting
        /// </summary>
        public SixnetFieldFormatSetting FormatSetting { get; set; }

        /// <summary>
        /// Gets or sets the database server
        /// </summary>
        public SixnetDatabaseServer Server { get; set; }

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
        public SixnetFieldLocation FieldLocation { get; set; } = SixnetFieldLocation.Output;

        /// <summary>
        /// Gets or sets the query location
        /// </summary>
        public SixnetQueryableLocation QueryLocation { get; set; } = SixnetQueryableLocation.Top;

        /// <summary>
        /// Resolve context
        /// </summary>
        public SixnetDataCommandResolveContext ResolveContext { get; set; }
    }
}
