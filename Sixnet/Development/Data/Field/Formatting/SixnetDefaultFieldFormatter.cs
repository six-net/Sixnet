// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Default default field formatter
    /// </summary>
    [Serializable]
    public class SixnetDefaultFieldFormatter : ISixnetFieldFormatter
    {
        readonly Func<SixnetFormatFieldContext, string> _fieldFormatDelegate = null;

        public SixnetDefaultFieldFormatter(Func<SixnetFormatFieldContext, string> fieldFormatDelegate)
        {
            if (fieldFormatDelegate is null)
            {
                throw new ArgumentNullException(nameof(fieldFormatDelegate));
            }
            _fieldFormatDelegate = fieldFormatDelegate;
        }

        public string Format(SixnetFormatFieldContext fieldConversionContext)
        {
            return _fieldFormatDelegate?.Invoke(fieldConversionContext);
        }
    }
}
