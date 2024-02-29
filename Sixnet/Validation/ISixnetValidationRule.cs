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
        ValidationField<T> Field { get; set; }
    }
}
