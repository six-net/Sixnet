namespace System
{
    /// <summary>
    /// Number extensions
    /// </summary>
    public static class NumberExtensions
    {
        #region Returns the smallest integral value that is greater than or equal to the specified double-precision number

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified double-precision number
        /// </summary>
        /// <param name="value">Double-precision number</param>
        /// <returns>Return the integral value</returns>
        public static long DoubleCeiling(this double value)
        {
            return Convert.ToInt64(Math.Ceiling(value));
        }

        #endregion

        #region Returns the largest integer that is less than or equal to the specified double-precision number

        /// <summary>
        /// Returns the largest integer that is less than or equal to the specified double-precision number
        /// </summary>
        /// <param name="value">Double-precision number</param>
        /// <returns>Return the integer value</returns>
        public static long DoubleFloor(this double value)
        {
            return Convert.ToInt64(Math.Floor(value));
        }

        #endregion

        #region Rounds a double-precision value to a specified number of 2 fractional digits

        /// <summary>
        /// Rounds a double-precision value to a specified number of fractional digits
        /// </summary>
        /// <param name="value">Double-precision nmumber</param>
        /// <returns>Return the number nearest to value that contains a number of fractional digits equal to digits</returns>
        public static double ToDouble(this object value)
        {
            double newDoubleValue;
            if (double.TryParse(value.ToString(), out newDoubleValue))
            {
                return Math.Round(newDoubleValue, 2);
            }
            return 0.00;
        }

        #endregion

        #region Returns the largest integral value that is greater than or equal to the calculate result by the specified double-precision number divide 1024

        /// <summary>
        /// Returns the largest integral value that is greater than or equal to the calculate result by the specified double-precision number divide 1024
        /// </summary>
        /// <param name="value">Double-precision number</param>
        /// <returns>Return the integral value</returns>
        public static double Divide1024(this double value)
        {
            double result = value / 1024;
            return Math.Ceiling(result);
        }

        #endregion

        #region Rounds the calculate result by two double-precision numbers divide to a specified number of 2 fractional digits

        /// <summary>
        /// Rounds the calculate result by two double-precision numbers divide to a specified number of 2 fractional digits
        /// </summary>
        /// <param name="value">Double-precision number</param>
        /// <param name="divideValue">Other double-precision number</param>
        /// <returns>Return the calculated value</returns>
        public static double Divide(this double value, double divideValue)
        {
            if (divideValue != 0)
            {
                return Math.Round(value / divideValue, 2);
            }
            return 0;
        }

        #endregion

        #region Calculate the height by the now-width and now-height scale value

        /// <summary>
        /// Calculate the height by the now width and now height scale value
        /// </summary>
        /// <param name="newWidth">New width</param>
        /// <param name="nowWidth">Now width</param>
        /// <param name="nowHeight">Now height</param>
        /// <returns>Return the new height value</returns>
        public static double ComputeHeight(this double newWidth, double nowWidth, double nowHeight)
        {
            #region verify args

            if (newWidth <= 0 || nowWidth <= 0 || nowHeight <= 0)
            {
                return 0.0;
            }

            #endregion

            double rate = Math.Round(nowWidth / nowHeight, 2);
            double newHeight = Math.Ceiling(newWidth / rate);
            return newHeight;
        }

        #endregion

        #region Calculate the width by the now-width and now-height scale value

        /// <summary>
        /// Calculate the width by the now-width and now-height scale value
        /// </summary>
        /// <param name="newHeight">New height</param>
        /// <param name="nowWidth">Now width</param>
        /// <param name="nowHeight">Now height`</param>
        /// <returns>Return thee new width value</returns>
        public static double ComputeWdith(this double newHeight, double nowWidth, double nowHeight)
        {
            #region verify args

            if (newHeight <= 0 || nowWidth <= 0 || nowHeight <= 0)
            {
                return 0.0;
            }

            #endregion

            double rate = Math.Round(nowWidth / nowHeight, 2);
            double newWidth = Math.Ceiling(newHeight * rate);
            return newWidth;
        }

        #endregion

        #region Rounds a decimal value to a specified value of 2 fractional digits

        /// <summary>
        /// Rounds a decimal value to a specified value of 2 fractional digits
        /// </summary>
        /// <param name="value">Decimal value</param>
        /// <returns>Return the round value</returns>
        public static decimal MathRound(this decimal value)
        {
            return Math.Round(value, 2);
        }

        /// <summary>
        /// Rounds a double-precision value to a specified value of 2 fractional digits
        /// </summary>
        /// <param name="value">Double-precision value</param>
        /// <returns>Return the round value</returns>
        public static double MathRound(this double value)
        {
            return Math.Round(value, 2);
        }

        #endregion
    }
}
