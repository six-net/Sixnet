namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field formatter contract
    /// </summary>
    public interface IFieldFormatter
    {
        /// <summary>
        /// Format field
        /// </summary>
        /// <param name="fieldConversionContext">Field conversion context</param>
        /// <returns>Return new field format</returns>
        string Format(FieldFormatContext fieldConversionContext);
    }
}
