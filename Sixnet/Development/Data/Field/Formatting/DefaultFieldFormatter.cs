using System;

namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Default default field formatter
    /// </summary>
    [Serializable]
    public class DefaultFieldFormatter : IFieldFormatter
    {
        readonly Func<FieldFormatContext, string> _fieldFormatDelegate = null;

        public DefaultFieldFormatter(Func<FieldFormatContext, string> fieldFormatDelegate)
        {
            if (fieldFormatDelegate is null)
            {
                throw new ArgumentNullException(nameof(fieldFormatDelegate));
            }
            _fieldFormatDelegate = fieldFormatDelegate;
        }

        public string Format(FieldFormatContext fieldConversionContext)
        {
            return _fieldFormatDelegate?.Invoke(fieldConversionContext);
        }
    }
}
