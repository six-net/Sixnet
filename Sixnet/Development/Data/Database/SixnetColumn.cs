// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Sixnet column
    /// </summary>
    public class SixnetColumn
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id {  get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Length
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Is fk
        /// </summary>
        public bool IsFk { get; set; }

        /// <summary>
        /// Increment
        /// </summary>
        public bool Increment { get; set; }

        /// <summary>
        /// Allow null
        /// </summary>
        public bool AllowNull { get; set; }

    }
}
