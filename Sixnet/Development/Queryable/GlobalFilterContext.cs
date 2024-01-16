using System;
using Sixnet.Development.Data;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Global filter context
    /// </summary>
    public class GlobalFilterContext
    {
        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        public Type ModelType { get; internal set; }

        /// <summary>
        /// Gets or sets the usage scene model type
        /// </summary>
        public Type UsageSceneModelType { get; internal set; }

        /// <summary>
        /// Gets or sets the original queryable
        /// </summary>
        public ISixnetQueryable OriginalQueryable { get; internal set; }

        /// <summary>
        /// Gets or sets the query location
        /// </summary>
        public QueryableLocation Location { get; internal set; }

        /// <summary>
        /// Gets or sets the operation type
        /// </summary>
        public DataOperationType OperationType { get; internal set; }
    }
}
