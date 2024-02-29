using System;
using Sixnet.Algorithm.Selection;

namespace Sixnet.Code
{
    /// <summary>
    /// Provides the functionality that generates the guid associated code
    /// </summary>
    public static class GuidHelper
    {
        /// <summary>
        /// Defines the collection used to generate random characters 
        /// </summary>
        private static readonly string ConstantCode;

        /// <summary>
        /// Provides an object that generates a random value
        /// </summary>
        private static readonly SixnetShuffleNet<char> ShuffleConstantCode;

        static GuidHelper()
        {
            ConstantCode = "1a2b3c4d5e6f7g8h9i0j9k8l7m6n5o4p3q2r1s0t1u2v3w4x5y6z".ToUpper();
            ShuffleConstantCode = new SixnetShuffleNet<char>(ConstantCode, true, true);
        }

        #region Gets a new guid value

        /// <summary>
        /// Gets a new guid value
        /// </summary>
        /// <returns>Return new guid value</returns>
        public static Guid GetGuid()
        {
            return Guid.NewGuid();
        }

        #endregion

        #region Gets an Int64 value generated from the guid

        /// <summary>
        /// Gets an Int64 value generated from the guid
        /// </summary>
        /// <returns>Retirm Int64 value</returns>
        public static long GetGuidInt64()
        {
            return Guid.NewGuid().ToInt64();
        }

        #endregion

        #region Gets an Int32 value generated from the guid

        /// <summary>
        /// Gets an Int32 value generated from the guid
        /// </summary>
        /// <returns>Return Int32 value</returns>
        public static int GetGuidInt32()
        {
            var firstValue = Guid.NewGuid();
            var firstNum = firstValue.ToInt32();
            var secondValue = Guid.NewGuid();
            var secondNum = secondValue.ToInt32();
            return Math.Abs(secondNum + firstNum);
        }

        #endregion

        #region Gets a string code generated from the guid

        /// <summary>
        /// Gets an string code generated from the guid
        /// </summary>
        /// <returns>Return guid unique code</returns>
        public static string GetGuidUniqueCode()
        {
            return Guid.NewGuid().ToUniqueCode();
        }

        #endregion

        #region Gets a specified length code by uniquecode

        /// <summary>
        /// Gets an specify length and prefix string code generated from the guid
        /// Recommend to set length at least 16
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="prefix">Prefix</param>
        /// <returns>Return guid unique code</returns>
        public static string GetGuidUniqueCode(int length, string prefix = "")
        {
            var uniqueCode = prefix + GetGuidUniqueCode().ToUpper();
            if (uniqueCode.Length == length)
            {
                return uniqueCode;
            }
            if (uniqueCode.Length > length)
            {
                uniqueCode = uniqueCode.Substring(0, length);
            }
            else
            {
                var outNum = length - uniqueCode.Length;
                uniqueCode += string.Join("", ShuffleConstantCode.TakeNextValues(outNum, true));
            }
            return uniqueCode;
        }

        #endregion
    }
}
