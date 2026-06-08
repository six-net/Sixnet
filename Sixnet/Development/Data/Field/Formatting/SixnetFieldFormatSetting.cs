// "Company © 2025. All rights reserved."

using Sixnet.Model;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field conversion options
    /// </summary>
    public class SixnetFieldFormatSetting : ISixnetCloneable<SixnetFieldFormatSetting>
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
        public SixnetFieldFormatSetting Child { get; private set; }

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
        public static SixnetFieldFormatSetting Create(string formatterName, object parameter = null)
        {
            return new SixnetFieldFormatSetting()
            {
                Name = formatterName,
                Parameter = parameter,
                HasDataField = parameter is SixnetDataField
            };
        }

        /// <summary>
        /// Set child field format setting
        /// </summary>
        /// <param name="child"></param>
        public void SetChild(SixnetFieldFormatSetting child)
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
        public SixnetFieldFormatSetting Clone()
        {
            return new SixnetFieldFormatSetting()
            {
                Name = Name,
                Parameter = Parameter,
                Child = Child?.Clone()
            };
        }
    }
}
