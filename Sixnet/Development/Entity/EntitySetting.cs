using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity setting
    /// </summary>
    public class EntitySetting
    {
        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// Key => Property name
        /// </summary>
        public Dictionary<string, FieldSetting> Fields { get; set; }
    }
}
