using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EntityRelationAttribute : Attribute
    {
        /// <summary>
        /// relation type
        /// </summary>
        public Type RelationType
        {
            get; set;
        }

        /// <summary>
        /// relation field
        /// </summary>
        public string RelationField
        {
            get; set;
        }

        /// <summary>
        /// init entity relation
        /// </summary>
        /// <param name="relationType">relation type</param>
        /// <param name="relationField">relation field</param>
        public EntityRelationAttribute(Type relationType, string relationField)
        {
            RelationType = relationType;
            RelationField = relationField;
        }
    }
}
