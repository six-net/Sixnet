// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity setting
    /// </summary>
    public class SixnetEntitySetting
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// Key => Property name
        /// </summary>
        public Dictionary<string, SixnetFieldSetting> Fields { get; set; }
    }
}
