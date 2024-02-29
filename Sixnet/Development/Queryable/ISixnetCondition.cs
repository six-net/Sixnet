namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines condition contract
    /// </summary>
    public interface ISixnetCondition
    {
        /// <summary>
        /// Gets the connector
        /// </summary>
        CriterionConnector Connector { get; set; }

        /// <summary>
        /// Whether is none
        /// </summary>
        bool None { get; }

        /// <summary>
        /// Whether negation
        /// </summary>
        bool Negation { get; }
    }
}
