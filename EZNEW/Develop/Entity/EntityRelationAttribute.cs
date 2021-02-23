using System;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity relation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class EntityRelationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the relation type
        /// </summary>
        public Type RelationType { get; set; }

        /// <summary>
        /// Gets or sets the relation field
        /// </summary>
        public string RelationField { get; set; }

        /// <summary>
        /// Initialize entity relation
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
