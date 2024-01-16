namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines criterion options
    /// </summary>
    public class CriterionOptions
    {
        /// <summary>
        /// Gets or sets the query field name used for subquery
        /// </summary>
        public string SubqueryField { get; set; }

        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public CriterionOptions Clone()
        {
            return new CriterionOptions()
            {
                SubqueryField = SubqueryField
            };
        }
    }
}
