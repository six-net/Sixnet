using System;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the field description
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the field is primary key
        /// </summary>
        public bool PrimaryKey
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the cache option
        /// </summary>
        public EntityFieldCacheOption CacheOption
        {
            get; set;
        } = EntityFieldCacheOption.None;

        /// <summary>
        /// Gets or sets whether disable the field in query
        /// </summary>
        public bool DisableQuery
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether disable the field in edit
        /// </summary>
        public bool DisableEdit
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the field is version field
        /// </summary>
        public bool IsVersion
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the field is refresh date field
        /// </summary>
        public bool RefreshDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the query format
        /// </summary>
        public string QueryFormat
        {
            get; set;
        }

        #endregion
    }
}
