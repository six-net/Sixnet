namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Global condition
    /// </summary>
    public class GlobalCondition
    {
        /// <summary>
        /// Gets or sets the globacl condition value
        /// </summary>
        public IQuery Value { get; set; }

        /// <summary>
        /// Gets or sets the global condition append method
        /// Default value is 'AND'
        /// </summary>
        public QueryOperator AppendMethod { get; set; } = QueryOperator.AND;

        /// <summary>
        /// Append global filter to origin query
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        public void AppendTo(IQuery originalQuery)
        {
            originalQuery?.SetGlobalCondition(Value, AppendMethod);
        }
    }
}
