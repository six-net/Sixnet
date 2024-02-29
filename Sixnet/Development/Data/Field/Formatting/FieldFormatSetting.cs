using Sixnet.Model;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field conversion options
    /// </summary>
    public class FieldFormatSetting : ISixnetCloneableModel<FieldFormatSetting>
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
        public FieldFormatSetting Child { get; set; }

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
                Parameter = parameter
            };
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
