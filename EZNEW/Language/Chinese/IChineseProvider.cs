using System.Collections.Generic;

namespace EZNEW.Language.Chinese
{
    /// <summary>
    /// Chinese provider contract
    /// </summary>
    public interface IChineseProvider
    {
        #region Gets the spelling from chinese string

        /// <summary>
        /// Get the spelling from chinese string
        /// </summary>
        /// <param name="chineseValue">Chinese value</param>
        /// <param name="split">Split string,default is a space</param>
        /// <param name="toLowerCase">Whether to lower,default is upper</param>
        /// <returns>Return the spelling</returns>
        string GetSpellingBySimpleChinese(string chineseValue, string split = " ", bool toLowerCase = false);

        #endregion

        #region Gets the spelling first char from chinese string

        /// <summary>
        /// Get the spelling first char from chinese string
        /// </summary>
        /// <param name="chineseValue">Chinese value</param>
        /// <param name="split">Split string</param>
        /// <param name="toLowerCase">Whether to lower,default is upper</param>
        /// <returns>Return the spelling first char</returns>
        string GetSpellingShortSimpleChinese(string chineseValue, string split = "", bool toLowerCase = false);

        #endregion

        #region Gets the spelling from chinese char

        /// <summary>
        /// Get the spelling from chinese char
        /// </summary>
        /// <param name="chineseChar">Chinese char</param>
        /// <returns>Return the spelling</returns>
        List<string> GetChineseCharSpellingList(char chineseChar);

        #endregion

        #region Chinese simple to traditional

        /// <summary>
        /// Convert simple chinese value to traditional
        /// </summary>
        /// <param name="simpleChineseValue">Chinese simple value</param>
        /// <returns>Return the traditional value</returns>
        string ConvertSimpleToTraditional(string simpleChineseValue);

        #endregion

        #region Convert traditional chinese value to simple

        /// <summary>
        /// Convert traditional chinese value to simple
        /// </summary>
        /// <param name="traditionalChineseValue">Chinese traditional value</param>
        /// <returns>Return tthe chinese simple value</returns>
        string TraditionalToSimple(string traditionalChineseValue);

        #endregion
    }
}
