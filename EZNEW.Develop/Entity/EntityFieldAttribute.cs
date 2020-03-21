using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFieldAttribute : Attribute
    {
        #region propertys

        /// <summary>
        /// name
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// description
        /// </summary>
        public string Description
        {
            get; set;
        }

        /// <summary>
        /// primary key
        /// </summary>
        public bool PrimaryKey
        {
            get; set;
        }

        /// <summary>
        /// cache option
        /// </summary>
        public EntityFieldCacheOption CacheOption
        {
            get; set;
        } = EntityFieldCacheOption.None;

        /// <summary>
        /// disable query
        /// </summary>
        public bool DisableQuery
        {
            get; set;
        }

        /// <summary>
        /// disable edit
        /// </summary>
        public bool DisableEdit
        {
            get; set;
        }

        /// <summary>
        /// version field
        /// </summary>
        public bool IsVersion
        {
            get; set;
        }

        /// <summary>
        /// refresh date
        /// </summary>
        public bool RefreshDate
        {
            get; set;
        }

        /// <summary>
        /// query format
        /// </summary>
        public string QueryFormat
        {
            get; set;
        }

        #endregion
    }
}
