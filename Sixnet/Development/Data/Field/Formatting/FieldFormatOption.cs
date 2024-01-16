using Sixnet.Model;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field conversion options
    /// </summary>
    public class FieldFormatOption : IInnerClone<FieldFormatOption>
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
        /// Gets or sets the child format option
        /// </summary>
        public FieldFormatOption ChildFormatOption { get; set; }

        /// <summary>
        /// Create a field format options
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public static FieldFormatOption Create(string formatterName, object parameter = null)
        {
            return new FieldFormatOption()
            {
                Name = formatterName,
                Parameter = parameter
            };
        }

        /// <summary>
        /// Clone conversion options
        /// </summary>
        /// <returns></returns>
        public FieldFormatOption Clone()
        {
            return new FieldFormatOption()
            {
                Name = Name,
                Parameter = Parameter,
                ChildFormatOption = ChildFormatOption?.Clone()
            };
        }
    }
}
