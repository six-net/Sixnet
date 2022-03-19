using EZNEW.Development.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Conversion
{
    /// <summary>
    /// Defines field conversion options
    /// </summary>
    public class FieldConversionOptions : IInnerClone<FieldConversionOptions>
    {
        /// <summary>
        /// Gets or sets the conversion name
        /// </summary>
        public string ConversionName { get; set; }

        /// <summary>
        /// Gets or sets the conversion parameter
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Create a field conversion options
        /// </summary>
        /// <param name="conversionName">Conversion name</param>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static FieldConversionOptions Create(string conversionName, object parameter = null)
        {
            return new FieldConversionOptions()
            {
                ConversionName = conversionName,
                Parameter = parameter
            };
        }

        /// <summary>
        /// Clone conversion options
        /// </summary>
        /// <returns></returns>
        public FieldConversionOptions Clone()
        {
            return new FieldConversionOptions()
            {
                ConversionName = ConversionName,
                Parameter = Parameter
            };
        }
    }
}
