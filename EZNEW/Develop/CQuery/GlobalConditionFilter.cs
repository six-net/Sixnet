using System;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Global condition filter
    /// </summary>
    public class GlobalConditionFilter
    {
        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        public Type EntityType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the usage scene entity type
        /// </summary>
        public Type UsageSceneEntityType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the original query
        /// </summary>
        public IQuery OriginalQuery
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the query source type
        /// </summary>
        public QuerySourceType SourceType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the query usage scene
        /// </summary>
        public QueryUsageScene UsageScene
        {
            get; set;
        }
    }
}
