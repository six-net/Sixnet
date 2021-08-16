using System;

namespace EZNEW.Code
{
    /// <summary>
    /// Provides the functionality for number
    /// </summary>
    public static class NumberHelper
    {
        #region Gets the maximum of a specified length

        /// <summary>
        /// Gets the maximum of a specified length
        /// 'numberLength' must be between 1 and 9
        /// </summary>
        /// <param name="numberLength">Number length(must be between 1 and 9)</param>
        /// <returns>Return the max number</returns>
        public static int GetMaximum(int numberLength)
        {
            if (numberLength < 1 || numberLength > 9)
            {
                throw new ArgumentException("Number length must be between 1 to 9");
            }
            if (numberLength == 9)
            {
                return 999999999;
            }
            var maxValue = Math.Pow(10, numberLength) - 1;
            return Convert.ToInt32(maxValue);
        }

        #endregion

        #region Gets the minimum of a specified length

        /// <summary>
        /// Gets the minimum of a specified length
        /// 'numberLength' must be between 1 to 10
        /// </summary>
        /// <param name="numberLength">Number length(must be between 1 to 10)</param>
        /// <returns>Return the minimum</returns>
        public static int GetMinimum(int numberLength)
        {
            if (numberLength < 1 || numberLength > 10)
            {
                throw new ArgumentException("Number length must be between 1 to 10");
            }
            if (numberLength == 1)
            {
                return 0;
            }
            return Convert.ToInt32(Math.Pow(10, numberLength - 1));
        }

        #endregion
    }
}
