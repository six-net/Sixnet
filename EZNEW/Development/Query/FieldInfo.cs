using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Data.Conversion;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines field info
    /// </summary>
    public class FieldInfo : IInnerClone<FieldInfo>
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the field conversion options
        /// </summary>
        public FieldConversionOptions ConversionOptions { get; set; }

        /// <summary>
        /// Indicates whether has conversion
        /// </summary>
        public bool HasConversion => !string.IsNullOrWhiteSpace(ConversionOptions?.ConversionName);

        /// <summary>
        /// Create a field info
        /// </summary>
        /// <param name="name">Field name</param>
        /// <param name="fieldConversionOptions">Field conversion options</param>
        /// <returns></returns>
        public static FieldInfo Create(string name, FieldConversionOptions fieldConversionOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Field name is null or empty");
            }
            return new FieldInfo()
            {
                Name = name,
                ConversionOptions = fieldConversionOptions
            };
        }

        public FieldInfo Clone()
        {
            return new FieldInfo()
            {
                Name = Name,
                ConversionOptions = ConversionOptions?.Clone()
            };
        }
    }
}
