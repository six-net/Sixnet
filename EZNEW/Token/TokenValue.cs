namespace EZNEW.Token
{
    /// <summary>
    /// Token value
    /// </summary>
    public class TokenValue
    {
        /// <summary>
        /// Gets or sets the the token string value
        /// </summary>
        public string StringValue
        {
            get;set;
        }

        /// <summary>
        /// Gets a empty token value
        /// </summary>
        /// <returns>return token value</returns>
        public static TokenValue Empty()
        {
            return new TokenValue()
            {
                StringValue = string.Empty
            };
        }
    }
}
