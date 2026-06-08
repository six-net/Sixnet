// "Company © 2025. All rights reserved."

namespace Sixnet.Token
{
    /// <summary>
    /// Token value
    /// </summary>
    public class SixnetTokenValue
    {
        /// <summary>
        /// Gets or sets the the token string value
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Gets a empty token value
        /// </summary>
        /// <returns>return token value</returns>
        public static SixnetTokenValue Empty()
        {
            return new SixnetTokenValue()
            {
                StringValue = string.Empty
            };
        }
    }
}
