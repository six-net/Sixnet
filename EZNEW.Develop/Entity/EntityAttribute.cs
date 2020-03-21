using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        #region Propertys

        /// <summary>
        /// entity's object name
        /// </summary>
        public string ObjectName
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
        /// entity group
        /// </summary>
        public string Group
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// entity description
        /// </summary>
        /// <param name="objectName">object name</param>
        /// <param name="group">entity group</param>
        /// <param name="desc">description</param>
        public EntityAttribute(string objectName, string group = "", string desc = "")
        {
            ObjectName = objectName;
            Group = group;
            Description = desc;
        }
    }
}
