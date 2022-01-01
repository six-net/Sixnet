namespace EZNEW.Data.Conversion
{
    /// <summary>
    /// Defines field converter contract
    /// </summary>
    public interface IFieldConverter
    {
        /// <summary>
        /// Convert field
        /// </summary>
        /// <param name="fieldConversionContext">Field conversion context</param>
        /// <returns>Return field conversion result</returns>
        FieldConversionResult Convert(FieldConversionContext fieldConversionContext);
    }
}
