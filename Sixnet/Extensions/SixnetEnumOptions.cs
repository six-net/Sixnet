using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Extensions
{
    /// <summary>
    /// Sixnet enum options
    /// </summary>
    public class SixnetEnumOptions
    {
        /// <summary>
        /// Not start by enum type name when output enum item name.
        /// Default is false.
        /// </summary>
        public bool NotStartByTypeName {  get; set; }

        /// <summary>
        /// Whether not output enum item display name.
        /// Default is false.
        /// </summary>
        public bool NotOutputDisplayName {  get; set; }

        /// <summary>
        /// Whether not upper item name
        /// Default is false
        /// </summary>
        public bool UppercaseName {  get; set; }

        /// <summary>
        /// Whether not separate name.
        /// Default is false
        /// </summary>
        public bool NotSeparateName { get; set; }

        /// <summary>
        /// Gets or sets the name separate char
        /// </summary>
        public string NameSeparateChar {  get; set; }

        internal string GetOptionsIdentityKey()
        {
            return $"{NotStartByTypeName}{NotOutputDisplayName}{UppercaseName}{NotSeparateName}{NameSeparateChar}";
        }
    }
}
