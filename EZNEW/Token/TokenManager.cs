using System;
using System.Collections.Generic;

namespace EZNEW.Token
{
    /// <summary>
    /// Token manager
    /// </summary>
    public static class TokenManager
    {
        /// <summary>
        /// Token engines
        /// </summary>
        static readonly Dictionary<string, ITokenEngine> TokenEngines = new Dictionary<string, ITokenEngine>();

        #region Encode

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="tokenOption">Token option</param>
        /// <returns>Return the token value</returns>
        public static TokenValue Encode(TokenOptions tokenOption)
        {
            var tokenEngine = GetTokenEngine(tokenOption?.TokenType ?? string.Empty);
            return tokenEngine.Encode(tokenOption);
        }

        #endregion

        #region Decode

        /// <summary>
        /// Decode token value
        /// </summary>
        /// <param name="tokenOption">Token option</param>
        /// <returns>Return token string</returns>
        public static string Decode(TokenOptions tokenOption)
        {
            var tokenEngine = GetTokenEngine(tokenOption?.TokenType ?? string.Empty);
            return tokenEngine.Decode(tokenOption);
        }

        /// <summary>
        /// Decode token to a dictionary
        /// </summary>
        /// <param name="tokenOption">Token option</param>
        /// <returns>Return dictionary value</returns>
        public static Dictionary<string, object> DecodeToDictionary(TokenOptions tokenOption)
        {
            var tokenEngine = GetTokenEngine(tokenOption?.TokenType ?? string.Empty);
            return tokenEngine.DecodeToDictionary(tokenOption);
        }

        /// <summary>
        /// Decode token to object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="tokenOption">Token option</param>
        /// <returns>Return data object</returns>
        public static T DecodeToObject<T>(TokenOptions tokenOption)
        {
            var tokenEngine = GetTokenEngine(tokenOption?.TokenType ?? string.Empty);
            return tokenEngine.DecodeToObject<T>(tokenOption);
        }

        #endregion

        #region Configure token engine

        /// <summary>
        /// Configure token engine
        /// </summary>
        /// <param name="tokenType">Token type</param>
        /// <param name="tokenEngine">Token engine</param>
        public static void ConfigureTokenEngine(string tokenType, ITokenEngine tokenEngine)
        {
            TokenEngines[tokenType] = tokenEngine;
        }

        #endregion

        #region Gets token engine

        /// <summary>
        /// Gets token engine by token type
        /// </summary>
        /// <param name="tokenType">Token type</param>
        /// <returns>Return token engine</returns>
        static ITokenEngine GetTokenEngine(string tokenType)
        {
            if (string.IsNullOrWhiteSpace(tokenType) || TokenEngines == null || TokenEngines.Count <= 0)
            {
                throw new Exception("Token engine is not found");
            }
            if (TokenEngines.ContainsKey(tokenType))
            {
                return TokenEngines[tokenType];
            }
            throw new Exception("Token engine is not found");
        }

        #endregion
    }
}
