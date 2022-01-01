using System;

namespace EZNEW.Data.Conversion
{
    /// <summary>
    /// Default field convert
    /// </summary>
    [Serializable]
    public class DefaultFieldConverter : IFieldConverter
    {
        Func<FieldConversionContext, FieldConversionResult> _fieldConversionDelegate = null;

        public DefaultFieldConverter(Func<FieldConversionContext, FieldConversionResult> fieldConversionDelegate)
        {
            if (fieldConversionDelegate is null)
            {
                throw new ArgumentNullException(nameof(fieldConversionDelegate));
            }
            _fieldConversionDelegate = fieldConversionDelegate;
        }

        public FieldConversionResult Convert(FieldConversionContext fieldConversionContext)
        {
            return _fieldConversionDelegate?.Invoke(fieldConversionContext);
        }
    }
}
