using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Data;

namespace EZNEW.Data.Conversion
{
    /// <summary>
    /// Defines conversion context
    /// </summary>
    public class FieldConversionContext
    {
        /// <summary>
        /// Gets or sets the conversion name
        /// </summary>
        public string ConversionName { get; set; }

        /// <summary>
        /// Gets or sets the database server
        /// </summary>
        public DatabaseServer Server { get; set; }

        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the conversion parameter
        /// </summary>
        public object Parameter { get; set; }
    }
}
