using System;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Default default field formatter
    /// </summary>
    [Serializable]
    public class DefaultFieldFormatter : ISixnetFieldFormatter
    {
        readonly Func<FormatFieldContext, string> _fieldFormatDelegate = null;

        public DefaultFieldFormatter(Func<FormatFieldContext, string> fieldFormatDelegate)
        {
            if (fieldFormatDelegate is null)
            {
                throw new ArgumentNullException(nameof(fieldFormatDelegate));
            }
            _fieldFormatDelegate = fieldFormatDelegate;
        }

        public string Format(FormatFieldContext fieldConversionContext)
        {
            return _fieldFormatDelegate?.Invoke(fieldConversionContext);
        }
    }
}
