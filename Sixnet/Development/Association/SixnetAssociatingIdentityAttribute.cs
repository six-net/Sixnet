// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Association
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetAssociatingIdentityAttribute : Attribute
    {
        public SixnetAssociatingIdentityAttribute(string associatedPropertyName = "Id", string associatedFieldName = "")
        {
            AssociatedPropertyName = associatedPropertyName;
            AssociatedFieldName = associatedFieldName;
        }

        /// <summary>
        /// Gets or sets the association property name
        /// </summary>
        internal string AssociatedPropertyName { get; set; }

        /// <summary>
        /// Gets or sets the association field name
        /// </summary>
        internal string AssociatedFieldName { get; set; }
    }
}
