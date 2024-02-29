using Sixnet.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sixnet.Token
{
    /// <summary>
    /// Token manager
    /// </summary>
    public static class TokenManager
    {
        /// <summary>
        /// Token providers
        /// </summary>
        static readonly ConcurrentDictionary<string, ITokenProvider> TokenProviders = new ConcurrentDictionary<string, ITokenProvider>();

        #region Encode

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return the token value</returns>
        public static TokenValue Encode(TokenOptions tokenOptions)
        {
            var tokenProvider = GetTokenProvider(tokenOptions?.TokenType ?? string.Empty);
            return tokenProvider.Encode(tokenOptions);
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decode token value
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return token string</returns>
        public static string Decode(TokenOptions tokenOptions)
        {
            var tokenProvider = GetTokenProvider(tokenOptions?.TokenType ?? string.Empty);
            return tokenProvider.Decode(tokenOptions);
        }

        /// <summary>
        /// Decode token to a dictionary
        /// </summary>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return dictionary value</returns>
        public static Dictionary<string, object> DecodeToDictionary(TokenOptions tokenOptions)
        {
            var tokenProvider = GetTokenProvider(tokenOptions?.TokenType ?? string.Empty);
            return tokenProvider.DecodeToDictionary(tokenOptions);
        }

        /// <summary>
        /// Decode token to object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="tokenOptions">Token options</param>
        /// <returns>Return data object</returns>
        public static T DecodeToObject<T>(TokenOptions tokenOptions)
        {
            var tokenProvider = GetTokenProvider(tokenOptions?.TokenType ?? string.Empty);
            return tokenProvider.DecodeToObject<T>(tokenOptions);
        }

        #endregion

        #region Add token provider

        /// <summary>
        /// Add token provider
        /// </summary>
        /// <param name="tokenType">Token type</param>
        /// <param name="tokenProvider">Token provider</param>
        public static void AddTokenProvider(string tokenType, ITokenProvider tokenProvider)
        {
            TokenProviders[tokenType] = tokenProvider;
        }

        #endregion

        #region Gets token provider

        /// <summary>
        /// Gets token provider by token type
        /// </summary>
        /// <param name="tokenType">Token type</param>
        /// <returns>Return token provider</returns>
        static ITokenProvider GetTokenProvider(string tokenType)
        {
            SixnetDirectThrower.ThrowSixnetExceptionIf(string.IsNullOrWhiteSpace(tokenType) || !TokenProviders.ContainsKey(tokenType), "Token provider is not found");
            return TokenProviders[tokenType];
        }

        #endregion
    }
}
