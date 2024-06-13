namespace Sixnet.Validation
{
    /// <summary>
    /// Defines validation rule contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISixnetValidationRule<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        SixnetValidationField<T> Field { get; set; }
    }
}
