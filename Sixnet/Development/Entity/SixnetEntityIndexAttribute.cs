// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet entity index attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SixnetEntityIndexAttribute() : Attribute
    {
        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public string[] Fields {  get; set; }

        /// <summary>
        /// Whether is a unique index
        /// </summary>
        public bool Unique {  get; set; }
    }
}
