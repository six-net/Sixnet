namespace Sixnet.Development.Data.Field.Formatting
{
    /// <summary>
    /// Defines field formatter contract
    /// </summary>
    public interface ISixnetFieldFormatter
    {
        /// <summary>
        /// Format field
        /// </summary>
        /// <param name="fieldConversionContext">Field conversion context</param>
        /// <returns>Return new field format</returns>
        string Format(FormatFieldContext fieldConversionContext);
    }
}
