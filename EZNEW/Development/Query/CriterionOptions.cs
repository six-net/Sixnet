using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Data.Conversion;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines criterion options
    /// </summary>
    public class CriterionOptions
    {
        /// <summary>
        /// Gets or sets the field conversion options
        /// </summary>
        public FieldConversionOptions FieldConversionOptions { get; set; }

        /// <summary>
        /// Gets or sets the query field name used for subquery
        /// </summary>
        public string SubqueryValueFieldName { get; set; }

        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public CriterionOptions Clone()
        {
            return new CriterionOptions()
            {
                FieldConversionOptions = FieldConversionOptions?.Clone()
            };
        }
    }
}
