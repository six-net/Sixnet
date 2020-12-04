namespace EZNEW.Token
{
    /// <summary>
    /// Token options
    /// </summary>
    public class TokenOptions
    {
        /// <summary>
        /// Gets or sets the token type
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the token object value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the secret
        /// </summary>
        public string Secret { get; set; }
    }
}
