// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity relation field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class SixnetEntityRelationFieldAttribute : Attribute
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
        public SixnetRelationBehavior Behavior { get; set; } = SixnetRelationBehavior.None;

        /// <summary>
        /// Initialize entity relation
        /// </summary>
        /// <param name="relationType">Relation type</param>
        /// <param name="relationField">Relation field</param>
        public SixnetEntityRelationFieldAttribute(Type relationType, string relationField, SixnetRelationBehavior behavior = SixnetRelationBehavior.None)
        {
            RelationType = relationType;
            RelationField = relationField;
            Behavior = behavior;
        }
    }

    /// <summary>
    /// Sixnet entity foreign key info
    /// </summary>
    public class SixnetEntityForeignKeyInfo
    {
        /// <summary>
        /// Gets or sets the source table 
        /// </summary>
        public SixnetDatabaseObjectName SourceTable { get; set; }

        /// <summary>
        /// Gets or sets the source field
        /// </summary>
        public SixnetDatabaseObjectName SourceField { get; set; }

        /// <summary>
        /// Gets or sets the reference table
        /// </summary>
        public SixnetDatabaseObjectName ReferenceTable { get; set; }

        /// <summary>
        /// Gets or sets the referebce field
        /// </summary>
        public SixnetDatabaseObjectName ReferenceField { get; set; }
    }
}
