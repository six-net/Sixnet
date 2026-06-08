// "Company © 2025. All rights reserved."

namespace Sixnet.Token
{
    /// <summary>
    /// Defines token provider
    /// </summary>
    public interface ISixnetTokenProvider
    {
        /// <summary>
        /// Encode token
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return token value</returns>
        SixnetTokenValue Encode(SixnetTokenOptions tokenOptions);

        /// <summary>
        /// Decode token value
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return token string</returns>
        string Decode(SixnetTokenOptions tokenOptions);

        /// <summary>
        /// Decode token to dictionary
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return dictionary value</returns>
        Dictionary<string, object> DecodeToDictionary(SixnetTokenOptions tokenOptions);

        /// <summary>
        /// Decode token to a object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return the token object</returns>
        T DecodeToObject<T>(SixnetTokenOptions tokenOptions);
    }
}
