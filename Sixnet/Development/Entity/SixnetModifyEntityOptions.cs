// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Modify entity options
    /// </summary>
    public class SixnetModifyEntityOptions
    {
        /// <summary>
        /// Whether include unmodifiable field
        /// </summary>
        public bool IncludeUnmodifiableField { get; set; }

        /// <summary>
        /// Ignore field names
        /// </summary>
        HashSet<string> _ignoreFieldNames;

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
            _ignoreFieldNames ??= new HashSet<string>();
            foreach (var fieldName in fieldNames)
            {
                _ignoreFieldNames.Add(fieldName);
            }
        }

        internal bool IsIgnoreField(SixnetDataField field)
        {
            return field == null
                || (field.IsUnmodifiableField() && !IncludeUnmodifiableField)
                || (_ignoreFieldNames?.Contains(field.PropertyName) ?? false);

        }
    }
}
