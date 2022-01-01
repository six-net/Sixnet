using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines global condition context
    /// </summary>
    public class GlobalConditionContext
    {
        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType { get; internal set; }

        /// <summary>
        /// Gets or sets the usage scene entity type
        /// </summary>
        public Type UsageSceneEntityType { get; internal set; }

        /// <summary>
        /// Gets or sets the original query
        /// </summary>
        public IQuery OriginalQuery { get; internal set; }

        /// <summary>
        /// Gets or sets the query location
        /// </summary>
        public QueryLocation Location { get; internal set; }

        /// <summary>
        /// Gets or sets the query usage scene
        /// </summary>
        public QueryUsageScene UsageScene { get; internal set; }
    }
}
