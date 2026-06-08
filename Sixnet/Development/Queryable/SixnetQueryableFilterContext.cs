// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Queryable filter context
    /// </summary>
    public class SixnetQueryableFilterContext
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
        public SixnetQueryableLocation Location { get; internal set; }

        /// <summary>
        /// Gets or sets the operation type
        /// </summary>
        public SixnetDataOperationType OperationType { get; internal set; }

        /// <summary>
        /// Gets or sets the operation options
        /// </summary>
        public SixnetDataOperationOptions OperationOptions { get; internal set; }
    }
}
