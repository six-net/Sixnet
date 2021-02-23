using System.Collections.Generic;

namespace EZNEW.Token
{
    /// <summary>
    /// Token provider contract
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Encode token
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return token value</returns>
        TokenValue Encode(TokenOptions tokenOptions);

        /// <summary>
        /// Decode token value
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return token string</returns>
        string Decode(TokenOptions tokenOptions);

        /// <summary>
        /// Decode token to dictionary
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return dictionary value</returns>
        Dictionary<string, object> DecodeToDictionary(TokenOptions tokenOptions);

        /// <summary>
        /// Decode token to a object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return the token object</returns>
        T DecodeToObject<T>(TokenOptions tokenOptions);
    }
}
