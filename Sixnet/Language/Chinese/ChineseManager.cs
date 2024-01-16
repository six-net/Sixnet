using System.Collections.Generic;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;

namespace Sixnet.Language.Chinese
{
    /// <summary>
    /// Chinese manager
    /// </summary>
    public static class ChineseManager
    {
        #region Get chinese provider

        /// <summary>
        /// Get chinese provider
        /// </summary>
        /// <returns>Return chinese provider</returns>
        public static IChineseProvider GetChineseProvider()
        {
            return ContainerManager.Resolve<IChineseProvider>();
        }

        /// <summary>
        /// Get chinese provider
        /// </summary>
        /// <returns>Return chinese provider</returns>
        static IChineseProvider GetChineseProviderThrowIfNull()
        {
            var chineseProvider = GetChineseProvider();
            if (chineseProvider == null)
            {
                throw new SixnetException("No Chinese provider is set");
            }
            return chineseProvider;
        }

        #endregion

        #region Gets the spelling from chinese string

        /// <summary>
        /// Get the spelling from chinese string
        /// </summary>
        /// <param name="chineseValue">Chinese value</param>
        /// <param name="split">Split string,default is a space</param>
        /// <param name="toLowerCase">Whether to lower,default is upper</param>
        /// <returns>Return the spelling</returns>
        public static string GetSpellingBySimpleChinese(string chineseValue, string split = " ", bool toLowerCase = false)
        {
            var chineseProvider = GetChineseProviderThrowIfNull();
            return chineseProvider.GetSpellingBySimpleChinese(chineseValue, split, toLowerCase);
        }

        #endregion

        #region Gets the spelling first char from chinese string

        /// <summary>
        /// Get the spelling first char from chinese string
        /// </summary>
        /// <param name="chineseValue">Chinese value</param>
        /// <param name="split">Split string</param>
        /// <param name="toLowerCase">Whether to lower,default is upper</param>
        /// <returns>Return the spelling first char</returns>
        public static string GetSpellingShortSimpleChinese(string chineseValue, string split = "", bool toLowerCase = false)
        {
            var chineseProvider = GetChineseProviderThrowIfNull();
            return chineseProvider.GetSpellingShortSimpleChinese(chineseValue, split, toLowerCase);
        }

        #endregion

        #region Gets the spelling from chinese char

        /// <summary>
        /// Get the spelling from chinese char
        /// </summary>
        /// <param name="chineseChar">Chinese char</param>
        /// <returns>Return the spelling</returns>
        public static List<string> GetChineseCharSpellingList(char chineseChar)
        {
            var chineseProvider = GetChineseProviderThrowIfNull();
            return chineseProvider.GetChineseCharSpellingList(chineseChar);
        }

        #endregion

        #region Chinese simple to traditional

        /// <summary>
        /// Convert simple chinese value to traditional
        /// </summary>
        /// <param name="simpleChineseValue">Chinese simple value</param>
        /// <returns>Return traditional value</returns>
        public static string ConvertSimpleToTraditional(string simpleChineseValue)
        {
            var chineseProvider = GetChineseProviderThrowIfNull();
            return chineseProvider.ConvertSimpleToTraditional(simpleChineseValue);
        }

        #endregion

        #region Convert traditional chinese value to simple

        /// <summary>
        /// Convert traditional chinese value to simple
        /// </summary>
        /// <param name="traditionalChineseValue">Chinese traditional value</param>
        /// <returns>Return the chinese simple value</returns>
        public static string TraditionalToSimple(string traditionalChineseValue)
        {
            var chineseProvider = GetChineseProviderThrowIfNull();
            return chineseProvider.TraditionalToSimple(traditionalChineseValue);
        }

        #endregion
    }
}
