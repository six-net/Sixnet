﻿using System.Collections.Generic;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Configuration
{
    /// <summary>
    /// Data entity configuration
    /// </summary>
    public class DataEntityConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public List<EntityField> Fields { get; set; }

        #endregion
    }
}
