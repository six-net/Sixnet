using System;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity relation field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class EntityRelationFieldAttribute : Attribute
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
        /// Gets or sets the relation behavior
        /// </summary>
        public RelationBehavior Behavior { get; set; } = RelationBehavior.None;

        /// <summary>
        /// Initialize entity relation
        /// </summary>
        /// <param name="relationType">Relation type</param>
        /// <param name="relationField">Relation field</param>
        public EntityRelationFieldAttribute(Type relationType, string relationField, RelationBehavior behavior = RelationBehavior.None)
        {
            RelationType = relationType;
            RelationField = relationField;
            Behavior = behavior;
        }
    }
}
