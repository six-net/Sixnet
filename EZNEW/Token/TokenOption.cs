namespace EZNEW.Token
{
    /// <summary>
    /// Token option
    /// </summary>
    public class TokenOption
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
