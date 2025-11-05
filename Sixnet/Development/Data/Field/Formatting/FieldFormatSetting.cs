// "Company © 2025. All rights reserved."

using Sixnet.Model;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field conversion options
    /// </summary>
    public class FieldFormatSetting : ISixnetCloneable<FieldFormatSetting>
    {
        /// <summary>
        /// Gets or sets the conversion name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the conversion parameter
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// Gets or sets the child format options
        /// </summary>
        public FieldFormatSetting Child { get; private set; }

        /// <summary>
        /// Gets or sets weather has data field
        /// </summary>
        public bool HasDataField { get; set; }

        /// <summary>
        /// Create a field format options
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static FieldFormatSetting Create(string formatterName, object parameter = null)
        {
            return new FieldFormatSetting()
            {
                Name = formatterName,
                Parameter = parameter,
                HasDataField = parameter is DataField
            };
        }

        /// <summary>
        /// Set child field format setting
        /// </summary>
        /// <param name="child"></param>
        public void SetChild(FieldFormatSetting child)
        {
            if (child != null)
            {
                Child = child;
                HasDataField |= child.HasDataField;
            }
        }

        /// <summary>
        /// Clone conversion options
        /// </summary>
        /// <returns></returns>
        public FieldFormatSetting Clone()
        {
            return new FieldFormatSetting()
            {
                Name = Name,
                Parameter = Parameter,
                Child = Child?.Clone()
            };
        }
    }
}
