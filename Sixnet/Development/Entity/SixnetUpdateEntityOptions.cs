// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Update entity options
    /// </summary>
    public class SixnetUpdateEntityOptions
    {
        /// <summary>
        /// Whether include default ignored role fields
        /// </summary>
        public bool IncludeDefaultIgnoredRoleFields { get; set; }

        /// <summary>
        /// Whether include unupdatable fields
        /// </summary>
        public bool IncludeUnupdatableFields { get; set; }

        /// <summary>
        /// Ignored field names
        /// </summary>
        HashSet<string> _ignoredFieldNames;

        /// <summary>
        /// Ignore fields
        /// </summary>
        /// <param name="fieldNames"></param>
        public void IgnoreFields(params string[] fieldNames)
        {
            if (fieldNames.IsNullOrEmpty())
            {
                return;
            }
            _ignoredFieldNames ??= new HashSet<string>();
            foreach (var fieldName in fieldNames)
            {
                _ignoredFieldNames.Add(fieldName);
            }
        }

        /// <summary>
        /// Whether is ignored field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal bool IsIgnoredField(SixnetDataField field)
        {
            return field == null
                || (field.Unupdatable && !IncludeUnupdatableFields)
                || (field.IsDefaultUnupdatableRoleField() && !IncludeDefaultIgnoredRoleFields)
                || (_ignoredFieldNames?.Contains(field.PropertyName) ?? false);

        }
    }
}
