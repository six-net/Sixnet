// "Company © 2025. All rights reserved."

namespace Sixnet.Model
{
    /// <summary>
    /// Exist value parameter
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class SixnetExistValueParameter<TValue, TId>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets the current id
        /// </summary>
        public TId CurrentId { get; set; }
    }
}
