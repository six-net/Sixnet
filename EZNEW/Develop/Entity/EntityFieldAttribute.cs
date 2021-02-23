using System;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EntityFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the field description
        /// </summary>
        public string Description { get; set; }

        ///// <summary>
        ///// Gets or sets whether the field is primary key
        ///// </summary>
        //public bool PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the cache role
        /// </summary>
        public FieldCacheRole CacheRole { get; set; } = FieldCacheRole.None;

        /// <summary>
        /// Gets or sets whether disable the field in query
        /// </summary>
        public bool DisableQuery { get; set; }

        /// <summary>
        /// Gets or sets whether disable the field in edit
        /// </summary>
        public bool DisableEdit { get; set; }

        ///// <summary>
        ///// Gets or sets whether the field is version field
        ///// </summary>
        //public bool IsVersion { get; set; }

        ///// <summary>
        ///// Gets or sets whether the field is refresh date field
        ///// </summary>
        //public bool RefreshDate { get; set; }

        /// <summary>
        /// Gets or sets the database type name
        /// </summary>
        public string DbTypeName { get; set; }

        /// <summary>
        /// Gets or sets the max length
        /// </summary>
        public int MaxLength { get; set; } = -1;

        /// <summary>
        /// Whether is fixed length
        /// </summary>
        public bool IsFixedLength { get; set; }

        /// <summary>
        /// Whether is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the query format
        /// </summary>
        public string QueryFormat { get; set; }

        /// <summary>
        /// Gets or sets the field role
        /// </summary>
        public FieldRole Role { get; set; } = FieldRole.None;
    }
}
