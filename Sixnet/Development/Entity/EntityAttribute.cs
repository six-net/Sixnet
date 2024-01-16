using System;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity module
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Gets or sets the table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the entity description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether enable cache
        /// </summary>
        public bool EnableCache { get; set; } = false;

        /// <summary>
        /// Gets or sets the split table type
        /// </summary>
        public SplitTableType SplitTableType { get; set; } = SplitTableType.None;

        /// <summary>
        /// Gets or sets the split table provider name
        /// </summary>
        public string SplitTableProviderName { get; set; }

        /// <summary>
        /// Gets or sets the entity style
        /// </summary>
        public EntityStyle Style { get; set; } = EntityStyle.Physical;

        #endregion

        static internal EntityAttribute Default = new();
    }
}
