using System;

namespace EZNEW.Code
{
    /// <summary>
    /// Provides the functionality that generates random number
    /// </summary>
    public static class RandomNumberHelper
    {
        /// <summary>
        /// Random object
        /// </summary>
        private static readonly Random random;

        #region Static constructor

        static RandomNumberHelper()
        {
            random = new Random();
        }

        #endregion

        #region Functions

        #region Gets a random number whose length is less than or equal to the specified max length

        /// <summary>
        /// Gets a random number whose length is less than or equal to the specified max length
        /// maxLength must be between 1 to 10
        /// it will return the int.MaxValue when 'maxLength'>=10
        /// number length must be equal to max Length when constraintMaxLength is true
        /// </summary>
        /// <param name="maxLength">Number max length(must be between 1 to 10)</param>
        /// <param name="constraintMaxLength">Whether constraint number length equal maxLength</param>
        /// <returns>Return a random number</returns>
        public static int GetRandomNumberByLength(int maxLength, bool constraintMaxLength = false)
        {
            if (maxLength <= 0)
            {
                return 0;
            }
            int maxValue = maxLength >= 10 ? int.MaxValue : NumberHelper.GetMaximum(maxLength);
            int minValue = constraintMaxLength ? NumberHelper.GetMinimum(maxLength) : 0;
            return random.Next(minValue, maxValue);
        }

        #endregion

        #region Gets a random number

        /// <summary>
        /// Gets a random number
        /// </summary>
        /// <param name="maximum">Maximum(include)</param>
        /// <param name="minimum">Minimum(include)</param>
        /// <returns>Return a radom number</returns>
        public static int GetRandomNumber(int maximum, int minimum = 0)
        {
            return random.Next(minimum, maximum + 1);
        }

        #endregion

        #endregion
    }
}
