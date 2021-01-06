using System;

namespace EZNEW.Develop.Entity
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
        /// Gets or sets the entity structure
        /// </summary>
        public EntityStructure Structure { get; set; } = EntityStructure.Normal;

        #endregion

        /// <summary>
        /// Initialize entity description
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <param name="group">Entity group</param>
        /// <param name="desc">Description</param>
        public EntityAttribute(string objectName, string group = "", string desc = "")
        {
            ObjectName = objectName;
            Group = group;
            Description = desc;
        }
    }
}
