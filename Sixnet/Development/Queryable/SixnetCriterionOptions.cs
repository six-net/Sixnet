// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines criterion options
    /// </summary>
    public class SixnetCriterionOptions
    {
        /// <summary>
        /// Gets or sets the query field name used for subquery
        /// </summary>
        public string SubqueryField { get; set; }

        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public SixnetCriterionOptions Clone()
        {
            return new SixnetCriterionOptions()
            {
                SubqueryField = SubqueryField
            };
        }
    }
}
