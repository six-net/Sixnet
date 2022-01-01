using System;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Entity attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the entity object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the entity description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the entity group
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Indicates whether enable cache
        /// </summary>
        public bool EnableCache { get; set; } = false;

        #endregion

        static internal EntityAttribute Default = new EntityAttribute();
    }
}
